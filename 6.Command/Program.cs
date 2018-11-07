using Common.Unit;
using FakeSSL;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6.Command
{
    class Program
    {
        static void Main(string[] args)
        {
            //var filepath = AppDomain.CurrentDomain.BaseDirectory + "log4net.config";
            //log4net.Config.XmlConfigurator.Configure(new FileInfo(filepath));
            LogHelper.Info("info");
            LogHelper.Error("error");
            LogHelper.Debug("debug");
            Console.ReadKey();
        }
    }
}
