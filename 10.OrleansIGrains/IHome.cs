using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace _10.OrleansIGrains
{
    public interface IHome : IGrainWithGuidKey
    {
        Task<string> SayHelloWorld(string str);
    }
}
