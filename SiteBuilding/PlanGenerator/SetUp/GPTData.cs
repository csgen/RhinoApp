using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanGenerator.Toolkit
{
    internal class GPTData
    {
        public string model { get; set; } = "gpt-3.5-turbo";
        public List<Message> messages { get; set; }
        //public int max_tokens { get; set; } = 7;
        public double temperature { get; set; } = 0.7;
        //public int top_p { get; set; } = 1;
        //public int n { get; set; } = 0;
        //public bool stream { get; set; } = false;
        //public int? logprobs { get; set; } = null;
        //public string stop { get; set; } = "\n";

        public GPTData(string prompt)
        {
            this.messages = new List<Message>();
            this.messages.Add(new Message(prompt));
        }
    }
    internal class Message
    {
        public string role { get; set; } = "assistant";
        public string content { get; set; }
        public Message(string prompt)
        {
            this.content = prompt;
        }
    }
}
