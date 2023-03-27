using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanGenerator
{
    public class myArgs : INotifyPropertyChanged
    {
        private string area;
        public string Area
        {
            get => area;
            set
            {
                area = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Area"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
