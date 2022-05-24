/// <summary>
/// Temporary comment
/// </summary>
partial class Build
{
    /// <summary>
    /// More comments
    /// </summary>
    [Parameter] readonly string NetlifySiteId;

    [Parameter][Secret] readonly string NetlifySiteAccessToken;

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
        .OnlyWhenDynamic(() => !string.IsNullOrWhiteSpace(NetlifySiteId), () => !string.IsNullOrWhiteSpace(NetlifySiteAccessToken))
        .Executes(async () =>
        {
            var netlifyClient = new NetlifyClient(NetlifySiteAccessToken);
            var rootDirectory = ArtifactsDirectory / "wwwroot";
            await netlifyClient.UpdateSiteAsync(rootDirectory, NetlifySiteId);
        });
}