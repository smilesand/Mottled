using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WinServer
{
    public class TestServer
    {
        readonly Timer _timer;
        readonly ILog _log = LogManager.GetLogger(typeof(TestServer));
        public TestServer()
        {
            _timer = new Timer(1000) { AutoReset = true };
            _timer.Elapsed += new ElapsedEventHandler(OnTick);
            _timer.Elapsed += (sender, eventArgs) => Console.WriteLine("It is {0} and all is well", DateTime.Now);
        }
        protected virtual void OnTick(object sender, ElapsedEventArgs e)
        {
            _log.Debug("Tick:" + DateTime.Now.ToLongTimeString());
        }
        public void Start()
        {
            _log.Info("Service is Started");
            _timer.Start();
        }
        public void Stop()
        {
            _log.Info("Service is Stopped");
            _timer.Stop();
        }
    }
}
