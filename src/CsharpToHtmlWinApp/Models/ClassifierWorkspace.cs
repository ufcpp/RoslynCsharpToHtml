using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System.IO;

namespace CsharpToHtmlWinApp.Models;

public class ClassfierWorkspace : IDisposable
{
    public string? CsprojPath { get; set; }

    private readonly MSBuildWorkspace _workspace;
    private Project? _project;

    public ClassfierWorkspace()
    {
        var instances = MSBuildLocator.QueryVisualStudioInstances();
        var instance = instances.MaxBy(x => x.Version);

        if (instance is null) throw new InvalidOperationException();

        //todo: show which version loaded
        System.Diagnostics.Debug.WriteLine(instance.Version);

        MSBuildLocator.RegisterInstance(instance);
        _workspace = MSBuildWorkspace.Create();
    }

    public bool IsOpen => _project is not null;

    public async ValueTask OpenProjectAsync()
    {
        if (CsprojPath is not { } proj) return;

        // need help: I want to know how to reload modified source codes.
        // Currently, this method close and reopen whole of the csproj.
        if (_project is not null)
        {
            _workspace.CloseSolution();
        }
        _project = await _workspace.OpenProjectAsync(proj);

        var projectDirectory = Path.GetDirectoryName(Path.GetFullPath(proj)) ?? "";

        Documents = _project?.Documents?
            .Select(x => new ClassfierDocument(projectDirectory, x))
            .Where(x => !x.IsGenerated)
            ?? Array.Empty<ClassfierDocument>();
    }

    public IEnumerable<ClassfierDocument> Documents { get; private set; } = Array.Empty<ClassfierDocument>();

    public string? Text { get; private set; }

    public void Dispose()
    {
        _workspace.Dispose();
    }
}
