using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6.Command.Model
{
    public class AnswerModel
    {
        public Guid OID { get; set; }
        public string answers { get; set; }
    }
    public class Answer
    {
        public int ID { get; set; }
        public string answer { get; set; }
    }
}
