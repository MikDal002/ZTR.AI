using Nuke.Common.Git;

[ShutdownDotNetAfterServerBuild]
[GitHubActions(
    "ForPR",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    OnPullRequestBranches = new[] { DevelopBranch, MasterBranch },
    PublishArtifacts = false, FetchDepth = 0,
    InvokedTargets = new[] { nameof(Tests) },
    CacheKeyFiles = new[] { "global.json", "source/**/*.csproj" },
    EnableGitHubToken = true)]
[GitHubActions(
    "Deploy",
    GitHubActionsImage.WindowsLatest, 
    ImportSecrets = new [] {nameof(NetlifySiteId), nameof(NetlifySiteAccessToken) },
    OnPushBranches = new[] { MasterBranch }, 
    PublishArtifacts = false, FetchDepth = 0,
    InvokedTargets = new[] { nameof(Tests), nameof(PushToNetlify) },
    CacheKeyFiles = new[] { "global.json", "source/**/*.csproj" },
    EnableGitHubToken = true)]
partial class Build : NukeBuild
{
    const string MasterBranch = "master";
    const string DevelopBranch = "develop";
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(UpdateBuildNumber = true)] readonly GitVersion GitVersion;

    #region Tests Data
    DotNetTestSettings TestSettings => new();
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
            DotNetTest(TestSettings
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .SetProjectFile(Solution));
        });
}
