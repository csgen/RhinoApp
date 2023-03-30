using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlanGenerator.Toolkit;
using System.Windows.Media.Imaging;

namespace PlanGenerator
{
    /// <summary>
    /// Interaction logic for ClrPln.xaml
    /// </summary>
    public partial class ClrPln : Page
    {
        public static Bitmap bitmap;
        public static Bitmap generated_img;
        public static ClrPln instance;
        public ClrPln()
        {
            InitializeComponent();
            instance = this;
        }
        private void CapnGen(object sender, RoutedEventArgs e)
        {
            Send();
        }

        private void ReGen(object sender, RoutedEventArgs e)
        {
            ReSend();
        }

        private void Send()
        {
            try
            {
                if (Toolkit.Access.checkAccessibility(@"\\AI01\Sheared_disk\04-AD-ClrPlan\logging.csv"))
                {
                    string prompt = "sitePlan";
                    string Nprompt = "";
                    GeneratePlane(prompt, Nprompt);

                }
                else
                {
                    MessageBox.Show("请连接ECADI内网");
                }
            }
            catch
            {
                MessageBox.Show("请以ECADI员工账号登录电脑");
            }
        }

        private void GeneratePlane(string prompt, string Nprompt)
        {
            ClrPln.bitmap = Toolkit.Capture.CaptureScreen_seg();
            var content = Toolkit.ImageProcess.ImgToBase64String(ClrPln.bitmap);
            Toolkit.DiffusionData data = new Toolkit.DiffusionData(prompt, Nprompt, content, 400, 200);

            string input = System.Text.Json.JsonSerializer.Serialize(data);
            //MessageBox.Show(input);
            //string url = "http://10.88.118.201:7861/controlnet/txt2img";
            string url = "http://10.88.118.237:7861/controlnet/txt2img";
            //string test_url = "http://127.0.0.1:8000/";
            //var response = MyRequest.PostRequest(test_url, input);
            
            var response = Toolkit.MyRequest.PostRequest(url, input);
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(response);
            var arr = dict["images"] as JArray;
            var ret = arr.Select(x => x.Value<string>()).ToList();

            System.Drawing.Bitmap response_bmp = Toolkit.ImageProcess.Base64StringToImage(ret[0]);
            ClrPln.generated_img = response_bmp;
            ClrPln.instance.capture.Source = Toolkit.ImageProcess.ConvertBitmap(response_bmp);
        }

        private void ReSend()
        {
            try
            {
                if (Toolkit.Access.checkAccessibility(@"\\AI01\Sheared_disk\04-AD-ClrPlan\logging.csv"))
                {
                    string prompt = "sitePlan";
                    string Nprompt = "";
                    ReGeneratePlane(prompt, Nprompt);
                }
                else
                {
                    MessageBox.Show("请连接ECADI内网");
                }
            }
            catch
            {
                MessageBox.Show("请以ECADI员工账号登录电脑");
            }
        }

        private void ReGeneratePlane(string prompt, string Nprompt)
        {
            var content = "";

            if (ClrPln.bitmap != null)
            {
                content = Toolkit.ImageProcess.ImgToBase64String(ClrPln.bitmap);
            }
            else
            {
                MessageBox.Show("请先点击生成总平面图", "Warning");
                return;
            }
            
            Toolkit.DiffusionData data = new Toolkit.DiffusionData(prompt, Nprompt, content, 400, 200);

            string input = System.Text.Json.JsonSerializer.Serialize(data);
            //MessageBox.Show(input);
            //string url = "http://10.88.118.201:7861/controlnet/txt2img";
            string url = "http://10.88.118.237:7861/controlnet/txt2img";
            //string test_url = "http://127.0.0.1:8000/";
            //var response = MyRequest.PostRequest(test_url, input);

            var response = Toolkit.MyRequest.PostRequest(url, input);
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(response);
            var arr = dict["images"] as JArray;
            var ret = arr.Select(x => x.Value<string>()).ToList();

            System.Drawing.Bitmap response_bmp = Toolkit.ImageProcess.Base64StringToImage(ret[0]);
            ClrPln.generated_img = response_bmp;
            ClrPln.instance.capture.Source = Toolkit.ImageProcess.ConvertBitmap(response_bmp);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG (.png)|*.png";
            bool? result = dlg.ShowDialog();

            if (result == true )
            {
                if (ClrPln.bitmap != null)
                {
                    string filename = dlg.FileName;
                    ClrPln.generated_img.Save(filename);
                }
                else
                {
                    MessageBox.Show("请先点击生成总平面图", "Warning");
                    return;
                }
            }
        }
    }
}
