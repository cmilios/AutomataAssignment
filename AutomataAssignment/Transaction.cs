using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataAssignment
{
    public class Transaction
    {
        public State StartingPoint { get; set; }
        public string CharInserted { get; set; }
        public State EndingPoint { get; set; }

        public override string ToString()
        {
            return $"StartingPoint: {StartingPoint.Id.ToString()},EndingPoint: {EndingPoint.Id.ToString()},CharInserted: {CharInserted}";
        }
    }
}
