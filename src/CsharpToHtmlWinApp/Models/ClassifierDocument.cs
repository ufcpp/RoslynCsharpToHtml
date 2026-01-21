using CSharpToHtml;
using Microsoft.CodeAnalysis;
using System.IO;

namespace CSharpToHtmlWinApp.Models;

public class ClassifierDocument
{
    private readonly string _projectDirectory;
    private readonly Document _document;

    public ClassifierDocument(string projectDirectory, Document document)
    {
        _projectDirectory = projectDirectory;
        _document = document;

        var path = document.FilePath;
        var name = path is null ? null : Path.GetRelativePath(_projectDirectory, path);

        ShortName = name;
        IsGenerated = string.IsNullOrEmpty(name)
            || name.StartsWith(@"obj\", StringComparison.Ordinal)
            || name.StartsWith(@"obj/", StringComparison.Ordinal);
    }

    public string? ShortName { get; }
    public bool IsGenerated { get; }

    public async Task<(string original, string html)> ClassifyAsync() => await _document.ToHtmlAsync(false);
}
