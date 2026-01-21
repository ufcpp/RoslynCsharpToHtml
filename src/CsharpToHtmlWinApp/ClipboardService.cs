using System.Windows;

namespace CSharpToHtmlWinApp;

public interface IClipboardService
{
    ValueTask WriteTextAsync(string text);
}

public class WpfClipboardService : IClipboardService
{
    public ValueTask WriteTextAsync(string text)
    {
        Clipboard.SetText(text);

        return default;
    }
}
