using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public static myArgs myArgs;
        public Profile()
        {
            InitializeComponent();
            myArgs = Resources["MyArgs"] as myArgs;
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

        private void Layers_Changed(object sender, TextChangedEventArgs e)
        {
            
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                myArgs.Layers = textBox.Text;
            }
            Rhino.RhinoDoc.SelectObjects += RhinoDoc_SelectObjects;
        }
        
        private void RhinoDoc_SelectObjects(object sender, RhinoObjectSelectionEventArgs e)
        {
            var objs = e.RhinoObjects;
            foreach (RhinoObject obj in objs)
            {
                var data = obj.UserDictionary; //可以直接从RhinoObject获得userDictionary或者data
                if (data.ContainsKey("AutoPlan"))
                {
                    if (data["AutoPlan"] as string == "BuildingClass")
                    {
                        
                    }
                }
            }
        }

        private void SdHeight_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                myArgs.TempSdHeight = textBox.Text;
            }
        }
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 使用正则表达式检查输入的字符是否为数字
            Regex regex = new Regex("^[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 禁止使用空格键
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
