using Avalonia.Controls;

namespace Avalonia.Controls.SelectingCanvas.Demo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}