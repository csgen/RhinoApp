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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlanGenerator
{
    /// <summary>
    /// Interaction logic for RoadGen.xaml
    /// </summary>
    public partial class RoadGen : Page
    {
        public RoadGen()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void stRd_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }
        private void SelectOuterPath(object sender, RoutedEventArgs e)
        {
            RhinoApp.RunScript("GetOuterPathCommand", true);
        }
        private void SelectMainPath(object sender, RoutedEventArgs e)
        {
            RhinoApp.RunScript("GetMainPathCommand", true);
        }
        private void SelectP2P_Path(object sender, RoutedEventArgs e)
        {
            RhinoApp.RunScript("GetP2P_PathCommand", true);
        }
        private void Generate(object sender, RoutedEventArgs e)
        {
            RhinoApp.RunScript("GeneratePathObjectCommand", true);
            //MainWindow.myArgs.GreenArea
        }
        private void AutoDrawP2P(object sender, RoutedEventArgs e)
        {
            RhinoApp.RunScript("AutoDraw", true);
        }

        private void AddPath(object sender, RoutedEventArgs e)
        {
            RhinoApp.RunScript("AddPath", true);
        }

        private void EditPath(object sender, RoutedEventArgs e)
        {
            MyLib.MyLib.WIP_Message();
            //RhinoApp.RunScript("EditPathCommand", true);
        }

        private void DrawOuterPath(object sender, RoutedEventArgs e)
        {
            MyLib.MyLib.WIP_Message();
        }

        private void DrawMainPath(object sender, RoutedEventArgs e)
        {
            MyLib.MyLib.WIP_Message();
        }

        private void stRd_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void ndRd_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void UpdateP2P_Path(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyLib.MyLib.P2P_PathWidth = e.NewValue;
        }
        private void UpdateOuterPathWidth(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyLib.MyLib.OuterPathWidth = e.NewValue;
        }

        private void UpdateMainPathWidth(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyLib.MyLib.MainPathWidth = e.NewValue;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MyLib.MyLib.WIP_Message();
        }

    }
}
