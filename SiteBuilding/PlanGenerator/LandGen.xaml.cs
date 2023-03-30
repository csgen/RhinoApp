using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlanGenerator
{
    /// <summary>
    /// Interaction logic for LandGen.xaml
    /// </summary>
    public partial class LandGen : Page
    {
        public LandGen()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void TestDraw(object sender, RoutedEventArgs e)
        {
            Rhino.RhinoApp.RunScript("TestDrawCommand", true);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            
        }

        private bool isUpdating=false;
        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isUpdating) return;
            if (e.OldValue == e.NewValue) return;
            

            isUpdating = true;
            MyLib.MyLib.testRadius = e.NewValue;
            Rhino.RhinoApp.RunScript("TestUpdateCommand", true);
            
            isUpdating = false;
        }

        private void GenerateLandscape(object sender, RoutedEventArgs e)
        {
            Rhino.RhinoApp.RunScript("GenerateLandscapeCommand", true);
            UpdateLandscapeData();
        }

        private void TreeRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double rSetting = e.NewValue;//用户设置的radius大小
            double oMin = 0; double oMax = 1;
            double nMin = 0.3; double nMax = 1;
            double radiusScale = (nMax - nMin) / (oMax - oMin) * (rSetting - oMin) + nMin;
            MyLib.MyLib.TreeScale = radiusScale;
            RhinoApp.RunScript("GenerateLandscapeCommand", true);
            UpdateLandscapeData();
        }

        private void TreeDensityChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double densitySetting = e.NewValue;//用户设置的疏密度0-1
            double oMin = 0; double oMax = 1;
            double nMin = 0.3; double nMax = 1;
            double radiusScale = (nMax - nMin) / (oMax - oMin) * (densitySetting - oMin) + nMin;
            MyLib.MyLib.TreeDensity = e.NewValue;
            RhinoApp.RunScript("GenerateLandscapeCommand", true);
            UpdateLandscapeData();
        }
        private void UpdateLandscapeData()
        {
            MainWindow.myArgs.GreenArea = string.Format("{0:0.00}㎡", MyLib.MyLib.GreenArea);
            MainWindow.myArgs.ConcentrationGreenArea = string.Format("{0:0.00}㎡", MyLib.MyLib.ConcentrationGreenArea);
            double r = MyLib.MyLib.GreenArea / MyLib.MyLib.LandArea;
            MainWindow.myArgs.GreenAreaRatio = string.Format("{0:P}", r);
        }
    }
}
