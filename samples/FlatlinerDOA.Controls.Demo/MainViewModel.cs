namespace FlatlinerDOA.Controls.Demo;

using System.Collections.ObjectModel;
using System.ComponentModel;

public class MainViewModel : INotifyPropertyChanged
{

    public MainViewModel()
    {
        for (int i = 0; i < 1000; i++)
        {
            this.Items.Add(i.ToString());
        }
    }
    private bool isChildRectangleSelected;

    public bool IsChildRectangleSelected
    {
        get
        {
            return this.isChildRectangleSelected;
        }
        set
        {
            if (this.isChildRectangleSelected != value)
            {
                this.isChildRectangleSelected = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsChildRectangleSelected)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<string> Items { get; init; } = new();
}
