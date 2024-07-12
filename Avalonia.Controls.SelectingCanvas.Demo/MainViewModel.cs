using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls.SelectingCanvas.Demo
{
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
}
