namespace FlatlinerDOA.Controls.Demo;

using Avalonia.Controls;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainViewModel();
    }
}
