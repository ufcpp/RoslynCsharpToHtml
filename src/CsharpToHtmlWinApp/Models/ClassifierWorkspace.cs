using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System.IO;

namespace CSharpToHtmlWinApp.Models;

public class ClassifierWorkspace : IDisposable
{
    public string? CsprojPath { get; set; }
    public bool CopyOnLoad { get; set; }

    private readonly MSBuildWorkspace _workspace;
    private Project? _project;

    public ClassifierWorkspace()
    {
        _workspace = MSBuildWorkspace.Create();
    }

    public bool IsOpen => _project is not null;

    public async ValueTask OpenProjectAsync()
    {
        if (CsprojPath is not { } project) return;

        // need help: I want to know how to reload modified source codes.
        // Currently, this method close and reopen whole of the csproj.
        if (_project is not null)
        {
            _workspace.CloseSolution();
        }
        _project = await _workspace.OpenProjectAsync(project);

        var projectDirectory = Path.GetDirectoryName(Path.GetFullPath(project)) ?? "";

        Documents = _project?.Documents?
            .Select(x => new ClassifierDocument(projectDirectory, x))
            .Where(x => !x.IsGenerated)
            ?? [];
    }

    public IEnumerable<ClassifierDocument> Documents { get; private set; } = [];

    public ClassifierDocument? FindDocument(string? shortName)
    {
        if (shortName is null) return null;
        return Documents.FirstOrDefault(x => x.ShortName == shortName);
    }

    public string? Text { get; private set; }

    public void Dispose()
    {
        _workspace.Dispose();
    }
}
