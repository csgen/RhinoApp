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
using Rhino;
using Rhino.DocObjects;
using Rhino.Input.Custom;

namespace PlanGenerator
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Page
    {
        public Profile()
        {
            InitializeComponent();
        }

        private void Draw_Profile(object sender, RoutedEventArgs e)
        {
            Rhino.RhinoApp.RunScript("ShowBuildingShadowCommand", true);
            //MainWindow.myArgs.Area = string.Format("{0:0.00}㎡", MyLib.MyLib.area);
        } 

        private void SelectOuterBoundary(object sender, RoutedEventArgs e)
        {
            Rhino.RhinoApp.RunScript("GetOuterPathCommand", true);
            MainWindow.myArgs.LandArea = string.Format("{0:0.00}㎡", MyLib.MyLib.LandArea);
            if (MyLib.MyLib.LandArea != 0)
                MainWindow.myArgs.AreaRatio = string.Format("{0:0.00}", MyLib.MyLib.area / MyLib.MyLib.LandArea);
        }

        private void SelectBuildingProfile(object sender, RoutedEventArgs e)
        {
            RhinoApp.RunScript("GetBuildingsCommand", true);
            MainWindow.myArgs.Area = string.Format("{0:0.00}㎡", MyLib.MyLib.area);
            if (MyLib.MyLib.LandArea != 0)
                MainWindow.myArgs.AreaRatio = string.Format("{0:0.00}", MyLib.MyLib.area / MyLib.MyLib.LandArea);
        }
        private void SitePlan_Optimization(object sender, RoutedEventArgs e)
        {
            MyLib.MyLib.WIP_Message();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyLib.MyLib.WIP_Message();
        }
    }
}
