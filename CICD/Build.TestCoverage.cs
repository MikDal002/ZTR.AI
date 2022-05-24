partial class Build
{
    Target TestCoverage => _ => _
        .DependsOn(Tests)
        .TriggeredBy(Tests)
        .OnlyWhenStatic(() => IsWindowsWhenReleaseOrAnyOsWhenOther())
        .Executes(() =>
        {
            var coverageTestSettings = TestSettings
                .SetConfiguration(Configuration.Debug)
                .SetProjectFile(Solution);
            DotNetTest(coverageTestSettings);

            var previousCoverageFileResult = string.Empty;
            CoverletTasks.Coverlet(s => s
                .SetFormat(CoverletOutputFormat.cobertura, CoverletOutputFormat.json)
                .CombineWith(TestsProjects, (settings, project) =>
                    PrepareCoverageSettingsForCoveringProject(project, settings, coverageTestSettings,
                        ref previousCoverageFileResult)
                )
            );

            ReportGeneratorTasks.ReportGenerator(s => s
                .SetTargetDirectory(TestResultDirectory / "report")
                .SetFramework("net6.0")
                .SetReports(TestResultDirectory.GlobFiles("**/*.cobertura.xml").Select(d => d.ToString())));
        });

    CoverletSettings PrepareCoverageSettingsForCoveringProject(Project project, CoverletSettings settings,
        DotNetTestSettings coverageTestSettings, ref string previousCoverageFileResult)
    {
        var assemblyPath = FindAssemblyForProject(project);
        var coverageResultDirectory = TestResultDirectory / project.Name;

        settings = settings
            .SetAssembly(assemblyPath)
            .SetOutput(coverageResultDirectory + "/")
            .SetTargetSettings(coverageTestSettings
                .EnableNoBuild()
                .SetProjectFile(project));

        settings = MergeCoverageResultsWithPreviousRun(previousCoverageFileResult, settings);
        previousCoverageFileResult = SetThresholdForLastRun(project, coverageResultDirectory, ref settings);

        return settings;
    }

    string SetThresholdForLastRun(Project project, AbsolutePath testResultFile, ref CoverletSettings settings)
    {
        if (TestsProjects.Select(d => d.ProjectId).Last() == project.ProjectId)
            settings = settings.SetThreshold(UnitTestCoverage_Minimum);
        string previousCoverageResult = testResultFile / "coverage.json";
        return previousCoverageResult;
    }

    static CoverletSettings MergeCoverageResultsWithPreviousRun(string previousCoverageResult,
        CoverletSettings settings)
    {
        if (!string.IsNullOrWhiteSpace(previousCoverageResult))
            settings = settings.SetMergeWith(previousCoverageResult);
        return settings;
    }

    AbsolutePath FindAssemblyForProject(Project project)
    {
        var projectName = $"**/{Configuration.Debug}/**/" + project.Name + ".dll";
        var first = SourceDirectory.GlobFiles(projectName).First();
        return first;
    }
}