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
            Rhino.RhinoApp.RunScript("SiteBuildingCommand", true);
        } 

        private void SelectOuterBoundary(object sender, RoutedEventArgs e)
        {
            Rhino.RhinoApp.RunScript("GetOuterPathCommand", true);
        }

        private void SelectBuildingProfile(object sender, RoutedEventArgs e)
        {
            RhinoApp.RunScript("GetBuildingsCommand", true);
        }
        private void SitePlan_Optimization(object sender, RoutedEventArgs e)
        {

        }
    }
}
