using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using System.IO;
using System.Text;

namespace FlatlinerDOA.Controls.Demo;

public partial class InkCanvasDemo : UserControl
{
    public InkCanvasDemo()
    {
        InitializeComponent();
    }

    public InkCanvas Ink => this.FindDescendantOfType<InkCanvas>()!;

    private async void SaveSvgClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var options = new FilePickerSaveOptions
        {
            Title = "Save SVG File",
            DefaultExtension = "svg",
            SuggestedFileName = "signature.svg",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("SVG File") { Patterns = new[] { "*.svg" }  }
            }
        };

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(options);
        if (file != null)
        {
            var svgElement = this.Ink.ToSvg(new InkSvgExportOptions { IncludeBackgroundColor = true });
            string svgString = svgElement.ToString();

            using var stream = await file.OpenWriteAsync();
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            await writer.WriteAsync(svgString);
        }
    }

    private void ClearClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Ink.Clear();
    }
}