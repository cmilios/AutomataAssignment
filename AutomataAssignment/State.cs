using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataAssignment
{
    public class State
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Final { get; set; }

        public bool Start { get; set; }

        public override string ToString()
        {
            return $"Id: {Id},Name: {Name},Final: {Final},Start: {Start}";
        }
    }
}
