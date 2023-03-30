using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PlanGenerator.Toolkit
{
    internal class Access
    {
        public static bool checkAccessibility(string path)
        {
            //string test_path = @"\\ecadi-svr05\结构专业软件安装程序\";
            //DirectoryInfo dir = new DirectoryInfo(test_path);
            //if (!dir.Exists)
            //{
            //    return false;
            //}
            //else
            //{
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                string value = "";
                value = value + DateTime.Now.ToString();
                value = value + "," + System.Environment.UserName.ToString();
                value = value + "," + Dns.GetHostName().ToString();
                value = value + "," + IPGlobalProperties.GetIPGlobalProperties().DomainName.ToString();
                value = value + "," + Dns.GetHostName().ToString();
                value = value + "," + Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
                writer.WriteLine(value);
            }
            return true;

        }
    }
}
