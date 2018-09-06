using Common.Unit;
using FakeSSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6.Command
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://118.25.179.160/api/WOApi/PostWOData";
            string PostJson = "[1,2,3,4,5,6,7,8,9]";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(PostJson);
            string httpResponse = string.Empty;
            int TimeOut = 6000;
            HttpHelper.HttpPost(url, data, out httpResponse, TimeOut);
            Console.WriteLine(httpResponse);
            Console.Read();
        }
    }
}
