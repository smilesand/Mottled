using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace WinServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //这是一个实用的服务框架
            //安装TopShelfService.exe install
            //卸载TopShelfService.exe uninstall
            //FileInfo fi = new FileInfo(".//log4net.config");
            FileInfo fi = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(fi);
            HostFactory.Run(x =>
            {
                x.Service<TestServer>(s =>
                {
                    s.ConstructUsing(name => new TestServer());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Sample Topshelf Host");
                x.SetDisplayName("Stuff");
                x.SetServiceName("Stuff");
                x.UseLog4Net();
            });
        }
    }
}
