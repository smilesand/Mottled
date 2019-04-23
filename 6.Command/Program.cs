using _6.Command.Model;
using Common.Unit;
using FakeSSL;
using log4net;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Data;

namespace _6.Command
{
    /// <summary>
    /// 2019年4月21日16:38:51 by manbu
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 5; i++)
            {
                Send();
            }
            Recive();
        }

        public static void Send()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "admin", Password = "admin", Port = 5672 };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("Answer", true, false, false, null);//这里第二个参数设置了队列持久化，服务重启后队列仍然存在。创建一个Answer队列
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;//消息持久化，确保rabbitmq服务器重启不丢失数据
                    //channel.BasicQos(0, 1, false);
                    string message = AddAnswer();
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("", "Answer", null, body);//向Answer队列中发送消息
                    Console.WriteLine("Send------- {0}", message);
                }
            }
        }

        public static void Recive()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "admin", Password = "admin", Port = 5672 };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "Answer", durable: true,exclusive: false,autoDelete: false,arguments: null);//获取的是Answer持久化消息。
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: "Answer", autoAck: false, consumer: consumer);//这里autoAck: false关闭自动回答，改为下文的手动回答，会过设置为True则为自动回答，下文会报错，需要将下文手动回答关掉。
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("Recive------- {0}", message);//打印消息
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);//这里为手动回执，默认是自动回执的。autoAck消息回执，向服务端发送回执服务端才会清除此条消息。防止消费者挂掉消息被清除。
                    };
                    Console.ReadLine();
                }
            }
        }

        public static string AddAnswer()
        {
            AnswerModel answerModel = new AnswerModel();
            List<Answer> answers = new List<Answer>();
            for (int i = 0; i < 10; i++)
            {
                Answer answer = new Answer();
                answer.ID = i;
                answer.answer = "答案"+i;
                answers.Add(answer);
            }
            answerModel.OID = Guid.NewGuid();
            answerModel.answers = JsonConvert.SerializeObject(answers);
            return JsonConvert.SerializeObject(answerModel);
        }



        //这是一个递归算法
        public static DataTable data = new DataTable();
        shu shu2 = new shu();
        public static shu GetShu(string OID, shu shu2)
        {

            DataRow[] dataRows = data.Select(" POID='" + OID + "'");
            if (dataRows.Length == 0)
            {
                return shu2;
            }
            for (int i = 0; i < dataRows.Length; i++)
            {
                shu shu3 = new shu();
                shu3.parentOID = dataRows[i]["parentOID"].ToString();
                shu2.sonOID.Add(shu3);
                //vs.Add(dataRows[i][0].ToString());
                GetShu(dataRows[i]["sonOID"].ToString(), shu3);
            }
            return shu2;
        }


        public class shu
        {
            public string parentOID { get; set; }
            public List<shu> sonOID { get; set; }
        }
    }
}
