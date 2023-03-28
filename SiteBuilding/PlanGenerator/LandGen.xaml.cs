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
    }
}
