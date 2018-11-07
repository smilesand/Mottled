using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace _6.Command
{
    public class UDPBase
    {
        private Socket client = null;
        private bool run = true;
        private IPEndPoint endpoint = null;
        private Thread td = null;
        public Action<string, string> MsgCome { get; set; }
        public UDPBase(string ip, string port)
        {
            if (client == null)
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                endpoint = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt32(port));
            }
        }


        public UDPBase(string port)
        {
            if (client == null)
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                endpoint = new IPEndPoint(IPAddress.Any, Convert.ToInt32(port));
            }
        }
        /// <summary>启动UDP服务</summary>
        public void Start()
        {
            client.Bind(endpoint);
            td = new Thread(new ThreadStart(RecieveMsg));
            td.IsBackground = true;
            td.Start();
        }
        /// <summary>停止UDP服务</summary>
        public void Stop()
        {
            run = false;
            Thread.Sleep(500);
            td.Abort();
            client.Close();
            client = null;
        }
        /// <summary>发送信息【ip=ip，port=端口，msg=发送的信息】</summary>
        public string SendMsg(string ip, string port, string msg)
        {
            EndPoint point = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt32(port));
            var buffer = Encoding.UTF8.GetBytes(msg);
            try
            {
                client.SendTo(buffer, buffer.Length, SocketFlags.None, point);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private void RecieveMsg()
        {
            while (run)
            {
                try
                {
                    EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号  
                    byte[] buffer = new byte[65535];
                    int length = client.ReceiveFrom(buffer, ref point);//接收数据报  
                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    MsgCome(point.ToString(), message);
                }
                catch { }
            }
        }
    }
}