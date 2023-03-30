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
        private string areaRatio;
        private string landArea;
        private string greenArea;
        private string concentrationGreenArea;
        private string greenAreaRatio;
        public string Area//计容面积
        {
            get => area;
            set
            {
                area = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Area"));
            }
        }
        public string AreaRatio
        {
            get => areaRatio;
            set
            {
                areaRatio = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AreaRatio"));
            }
        }
        public string LandArea
        {
            get => landArea;
            set
            {
                landArea = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LandArea"));
            }
        }
        public string GreenArea
        {
            get => greenArea;
            set
            {
                greenArea = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GreenArea"));

            }
        }
        public string ConcentrationGreenArea
        {
            get => concentrationGreenArea;
            set
            {
                concentrationGreenArea = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConcentrationGreenArea"));
            }
        }
        public string GreenAreaRatio
        {
            get => greenAreaRatio;
            set
            {
                greenAreaRatio = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GreenAreaRatio"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
