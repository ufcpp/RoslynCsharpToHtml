using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CSharpToHtmlWinApp;

public partial class MainWindow : Window
{
    static MainWindow()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWpfBlazorWebView();
        serviceCollection.AddSingleton<Models.ClassifierWorkspace>();
        serviceCollection.AddSingleton<IClipboardService, WpfClipboardService>();
        Services = serviceCollection.BuildServiceProvider();
    }

    public MainWindow()
    {
        InitializeComponent();
    }

    public static ServiceProvider Services { get; }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        var settings = Settings.Load();

        if (settings is not null)
        {
            var workspace = Services.GetService<Models.ClassifierWorkspace>()!;
            workspace.CsprojPath = settings.CsprojPath;
            workspace.CopyOnLoad = settings.CopyOnLoad;
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        var workspace = Services.GetService<Models.ClassifierWorkspace>()!;
        Settings.Save(new(workspace.CsprojPath, workspace.CopyOnLoad));
        base.OnClosed(e);
    }
}
