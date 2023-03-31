using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PlanGenerator.Toolkit;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;


namespace PlanGenerator
{
    /// <summary>
    /// Interaction logic for DesignDes.xaml
    /// </summary>
    public partial class DesignDes : Page
    {
        public DesignDes()
        {
            InitializeComponent();
        }

        private void GenerateGPT(object sender, RoutedEventArgs e)
        {
            MyLib.MyLib.WIP_Message();
            #region 暂时在CY电脑上演示
            ////string value = "总体布局";
            ////if(value)
            //double totalArea = 125430.6;
            //int building_count = 2;
            //double building_height_1 = 41.2;
            //double building_height_2 = 50.7;
            //double road_width = 8;
            //string value = string.Format("项目总面积是{0}平方米，容积率是2.0。一共有{1}栋建筑，分别高{2}米和{3}米。项目内车行道宽度为{4}米。", 
            //    totalArea,building_count, building_height_1, building_height_2, road_width);
            
            
            //string text = "从总体布局、交通组织、立面设计三个方面写一份总平面设计说明。一定要使用我给你的全部数据，而且不要重复我的话";
            //string prompt = this.prompt.Text + value + text;
            ////prompt = "你好";
            ////MessageBox.Show(prompt);
            ////string prompt = this.prompt.Text;
            //Send(prompt);
            #endregion
        }

        void Send(string _prompt)
        {
            string API_KEY = "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";
            string prompt = _prompt;
             GPTData myData = new GPTData(prompt);
            var input = System.Text.Json.JsonSerializer.Serialize(myData);

            var response = MyRequest.PostRequest("https://api.openai.com/v1/chat/completions", input, API_KEY);
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(response);
            var message = dict["choices"] as JArray;
            var output = message[0]["message"]["content"];

            this.output.Text = output.ToString();
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.output.Text);
        }
    }
}
