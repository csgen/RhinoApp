using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanGenerator.Toolkit
{ 
    public class MyData
    {
        public string prompt { get; set; }
        public string negative_prompt { get; set; } = "";
        public int seed { get; set; } = -1;
        public int subseed { get; set; } = -1;
        public double subseed_strength { get; set; } = 0;
        public int batch_size { get; set; } = 1;
        public int n_iter { get; set; } = 1;
        public int steps { get; set; } = 20;
        public int cfg_scale { get; set; } = 7;
        public int width { get; set; } = 512;
        public int height { get; set; } = 512;
        public bool restore_faces { get; set; } = true;
        public int eta { get; set; } = 0;
        public string sampler_index { get; set; } = "DDIM";
        public List<string> controlnet_input_image { get; set; }

        //public string controlnet_module { get; set; } = "canny";
        public string controlnet_module { get; set; }

        //public string controlnet_model { get; set; } = "control_canny-fp16 [e3fe7712]";
        public string controlnet_model { get; set; }

        //public int controlnet_weight { get; set; } = 1;
        public double controlnet_weight { get; set; }
        public bool controlnet_lowvram { get; set; } = false;

        public MyData(string prompt, string Nprompt, string preprocessor, string model, double weight, string data, int width,int height)
        {
            this.prompt = prompt;

            this.negative_prompt = Nprompt;

            this.width = width;

            this.height = height;

            this.controlnet_input_image = new List<string>() { data };

            this.controlnet_module = preprocessor;

            this.controlnet_model = model;

            this.controlnet_weight = weight;
        }

    }
}
