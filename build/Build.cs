using System;
using System.Linq;

using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MinVer;

using Serilog;

using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push, },
    InvokedTargets = new[] { nameof(Clean), nameof(PublishToNuget) },
    AutoGenerate = true,
    FetchDepth = 0,
    ImportSecrets = new[] { "NUGET_API_KEY" })]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Publish);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter]
    readonly string NugetApiKey;

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    [MinVer]
    readonly MinVer MinVer;

    readonly AbsolutePath OutputPath = RootDirectory / "out";

    Target Clean => _ => _
        .Executes(() =>
        {
            OutputPath.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(settings => settings
                .SetProjectFile(Solution.Rtl8812auNet));
        });

    //Target Compile => _ => _
    //    .DependsOn(Restore)
    //    .Executes(() =>
    //    {
    //        DotNetBuild(settings => settings
    //            .SetNoRestore(true)
    //            .SetProjectFile(Solution.Rtl8812auNet)
    //            .SetConfiguration(Configuration));
    //    });

    Target Publish => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetPack(settings => settings
                .SetConfiguration(Configuration)
                //.SetNoBuild(true)
                .SetProject(Solution.Rtl8812auNet)
                .SetOutputDirectory(OutputPath)
                .SetVersion(MinVer.PackageVersion)
                .SetIncludeSymbols(true)
                .SetDeterministic(true)
                .SetNoBuild(false)
                .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg));
        });

    Target PublishToNuget => _ => _
        .DependsOn(Publish)
        .OnlyWhenDynamic(() => string.IsNullOrWhiteSpace(MinVer.MinVerPreRelease))
        .Executes(() =>
        {
            Log.Logger.Error(MinVer.MinVerPreRelease);
            DotNetNuGetPush(settings => settings
                .SetApiKey(NugetApiKey)
                .SetTargetPath(OutputPath.GlobFiles("*.nupkg").First())
                .SetSource("https://api.nuget.org/v3/index.json"));

            //DotNetNuGetPush(settings => settings
            //    .SetApiKey(NugetApiKey)
            //    .SetTargetPath(OutputPath.GlobFiles("*.snupkg").First())
            //    .SetSource("https://api.nuget.org/v3/index.json"));
        });
}
