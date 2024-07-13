namespace FlatlinerDOA.Controls.Demo;

using System.ComponentModel;

public class MainViewModel : INotifyPropertyChanged
{
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
}
