using System.Windows;
using System.Windows.Input;

namespace PlanGenerator
{
    public partial class MainWindow : Window
    {
        public static myArgs totalArea;
        public MainWindow()
        {
            InitializeComponent();
            totalArea = Resources["totalArea"] as myArgs;
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void BtnClickRoadGenerator(object sender, RoutedEventArgs e)
        {
            Main.Content = new RoadGen();
        }

        private void btnClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnClickProfile(object sender, RoutedEventArgs e)
        {
            Main.Content = new Profile();
        }

        private void BtnClickLandGen(object sender, RoutedEventArgs e)
        {
            Main.Content = new LandGen();
        }

        private void BtnClickClrPln(object sender, RoutedEventArgs e)
        {
            Main.Content = new ClrPln();
        }

        private void BtnClickDesignDes(object sender, RoutedEventArgs e)
        {
            Main.Content = new DesignDes();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
        public int i = 0;
        private void BindingTest(object sender, RoutedEventArgs e)
        {
            myArgs m = new myArgs();
            i++;
            m.Area = i.ToString();
        }
    }
}