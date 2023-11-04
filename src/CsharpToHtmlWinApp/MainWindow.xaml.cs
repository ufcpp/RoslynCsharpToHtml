using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CsharpToHtmlWinApp;

public partial class MainWindow : Window
{
    static MainWindow()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWpfBlazorWebView();
        serviceCollection.AddSingleton<Models.ClassfierWorkspace>();
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
            var workspace = Services.GetService<Models.ClassfierWorkspace>()!;
            workspace.CsprojPath = settings.CsprojPath;
            workspace.CopyOnLoad = settings.CopyOnLoad;
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        var workspace = Services.GetService<Models.ClassfierWorkspace>()!;
        Settings.Save(new(workspace.CsprojPath, workspace.CopyOnLoad));
        base.OnClosed(e);
    }
}
