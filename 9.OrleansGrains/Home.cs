using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _10.OrleansIGrains;
using Orleans;

namespace _9.OrleansGrains
{
    public class Home : Grain, IHome
    {
        public Task<string> SayHelloWorld(string str)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-" + str);
            return Task.FromResult<string>("完成");
        }
    }
}
