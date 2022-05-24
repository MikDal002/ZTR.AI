partial class Build
{
    [Parameter("Netlify site id")] readonly string NetlifySiteId;

    [Parameter("Netlify access token")] readonly string NetlifySiteAccessToken;

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