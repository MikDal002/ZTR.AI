using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetlifySharp;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using System.Linq;
using System.Runtime.InteropServices;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Logger = Serilog.Core.Logger;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[GitHubActions(
    "ForPR",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    OnPushBranchesIgnore = new[] { MasterBranch },
    OnPullRequestBranches = new[] { DevelopBranch },
    PublishArtifacts = false,
    InvokedTargets = new[] { nameof(Tests) },
    CacheKeyFiles = new[] { "global.json", "source/**/*.csproj" },
    EnableGitHubToken = true)]
[GitHubActions(
    "Deploy",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = new[] { MasterBranch },
    PublishArtifacts = false,
    InvokedTargets = new[] { nameof(Tests) },
    CacheKeyFiles = new[] { "global.json", "source/**/*.csproj" },
    EnableGitHubToken = true)]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    const string MasterBranch = "master";
    const string DevelopBranch = "develop";

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Netlify site id")]
    readonly string NetlifySiteId;

    [Parameter("Netlify access token")]
    readonly string NetlifySiteAccessToken;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;
    
    #region Tests Data
    readonly int UnitTestCoverage_Minimum = 69;

    DotNetTestSettings TestSettings => new DotNetTestSettings()
        .EnableCollectCoverage() // you need this line to coverage just started
        .SetConfiguration(Configuration)
        .SetResultsDirectory(TestResultDirectory / "results")
        .EnableNoBuild();
    IEnumerable<Project> TestsProjects => Solution.GetProjects("*.Test*");

    AbsolutePath TestResultDirectory => RootDirectory / "testrestults";

    #endregion

    AbsolutePath SourceDirectory => RootDirectory;
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Solution.GetProjects("ZTR.AI").ForEach(d => GlobDirectories(d.Directory, "**/bin", "**/obj").ForEach(DeleteDirectory));
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Clean, Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Tests => _ => _
        .DependsOn(Compile)
        .TriggeredBy(Compile)
        .Executes(() =>
        {
            EnsureCleanDirectory(TestResultDirectory);
            DotNetTest(TestSettings.SetProjectFile(Solution));
        });

    Target TestCoverage => _ => _
        .DependsOn(Tests)
        .TriggeredBy(Tests)
        .OnlyWhenStatic(() => IsWindowsWhenReleaseOrAnyOsWhenOther())
        .Executes(() =>
        {
            Serilog.Log.Warning($"IsWin: {RuntimeInformation.IsOSPlatform(OSPlatform.Windows)}, configuration: {Configuration}");
            var previousCoverageResult = string.Empty;
            CoverletTasks.Coverlet(s => s
                .SetFormat(CoverletOutputFormat.cobertura, CoverletOutputFormat.json)
                .CombineWith(TestsProjects, (settings, project) =>
                    {
                        var projectName = "**/" + project.Name + ".dll";
                        var first = SourceDirectory.GlobFiles(projectName).First();
                        var testResultFile = TestResultDirectory / project.Name; // / $"{project.Name}.cobertura.xml";

                        settings = settings.SetAssembly(first)
                            .SetTargetSettings(TestSettings.SetProjectFile(project))
                            .SetOutput(testResultFile + "/");

                        if (!string.IsNullOrWhiteSpace(previousCoverageResult))
                            settings = settings
                                .SetMergeWith(previousCoverageResult);
                        if (TestsProjects.Select(d => d.ProjectId).Last() == project.ProjectId)
                            settings = settings.SetThreshold(UnitTestCoverage_Minimum);

                        previousCoverageResult = testResultFile / "coverage.json";
                        return settings;
                    }

                )
            );

            ReportGeneratorTasks.ReportGenerator(s => s
                .SetTargetDirectory(TestResultDirectory / "report")
                .SetFramework("net6.0")
                .SetReports(TestResultDirectory.GlobFiles("**/*.cobertura.xml").Select(d => d.ToString())));
        });

    bool IsWindowsWhenReleaseOrAnyOsWhenOther()
    {
        var isWindows = IsWindows();
        if (isWindows && Configuration == Configuration.Release) return true;
        return Configuration.Release != Configuration;
    }

    static bool IsWindows()
    {
        try
        {
            Process p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "uname",
                    Arguments = "-s"
                }
            };
            p.Start();
            string uname = p.StandardOutput.ReadToEnd().Trim();
            Serilog.Log.Warning($"You run this built on {uname} machine.");
            // MSYS_NT - this name return uname on Github Action's machine.
            return uname.Contains("MSYS_NT", StringComparison.InvariantCultureIgnoreCase);
        }
        catch (Exception)
        {
            return true;
        }
    }

    Target Publish => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var projectToPublish = Solution.GetProject("ZTR.AI.Example");
            DotNetPublish(s => s
                .SetProject(projectToPublish)
                .SetConfiguration(Configuration)
                .SetOutput(ArtifactsDirectory));
        });

    Target PushToNetlify => _ => _
        .DependsOn(Publish)
        .Requires(() => NetlifySiteId, () => NetlifySiteAccessToken)
        .Executes(async () =>
        {

            var netlifyClient = new NetlifyClient(NetlifySiteAccessToken);
            var rootDirectory = ArtifactsDirectory / "wwwroot";
            await netlifyClient.UpdateSiteAsync(rootDirectory, NetlifySiteId);
        });
}
