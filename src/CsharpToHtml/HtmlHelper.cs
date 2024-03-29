using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace CsharpToHtml;

public static class HtmlHelper
{
    public static async Task<(string original, string html)> ToHtmlAsync(this Document doc, bool useStyle)
    {
        var builder = Tag.GetBuilder();

        var text = await doc.GetTextAsync();
        var classifiedSpans = await Classifier.GetClassifiedSpansAsync(doc, TextSpan.FromBounds(0, text.Length));
        builder.Append(classifiedSpans);

        if (await doc.Project.GetCompilationAsync() is { } compilation)
        {
            var diags = compilation.GetDiagnostics().Where(d => d.Location.SourceTree?.FilePath == doc.FilePath).ToArray();
            builder.Append(diags);
        }

        return (text.ToString(), text.ToHtml(builder.Build(), useStyle));
    }

    public static string ToHtml(this SourceText text, Tag.Queue tags, bool useStyle)
        => ToHtml(text.ToString(), tags, useStyle);

    public static string ToHtml(ReadOnlySpan<char> text, Tag.Queue tags, bool useStyle)
    {
        var s = new StringBuilder();

        s.Append(ClassTable.Header);

        for (int i = 0; i < text.Length; i++)
        {
            var c = text[i];

            while (tags.TryGet(i, out var tag))
            {
                if (tag.ClassName is { } @class)
                {
                    s.Append("<span");

                    if (useStyle && ClassTable.ClassToColor(@class) is { } color)
                    {
                        s.Append($"""
                             style="color:#{color};"
                            """);
                    }
                    else
                    {
                        s.Append($"""
                             class="{@class}"
                            """);
                    }

                    if (tag.DiagnosticId is { } id)
                    {
                        s.Append($"""
                             title="{id}"
                            """);
                    }

                    s.Append('>');
                }
                else
                {
                    s.Append("</span>");
                }
            }

            s.AppendEscape(c);
        }

        s.Append(ClassTable.Footer);

        return s.ToString();
    }

    public static void AppendEscape(this StringBuilder builder, char c)
    {
        switch (c)
        {
            case '<':
                builder.Append("&lt;");
                break;
            case '>':
                builder.Append("&gt;");
                break;
            case '&':
                builder.Append("&amp;");
                break;
            case '"':
                builder.Append("&quot;");
                break;
            default:
                builder.Append(c);
                break;
        }
    }
}
