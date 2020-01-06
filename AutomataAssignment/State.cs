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

        public List<Transaction> AbleTransactions{get; set;}

        public List<Transaction> TransactionCameFrom { get; set; }

        public override string ToString()
        {
            return $"Id: {Id},Name: {Name},Final: {Final},Start: {Start}";
        }

        public List<State> Resolve(string letter)
        {
            var possibleStates = new List<State>();
            if(AbleTransactions.Where(x=> x.CharInserted == letter).ToList().Any())
            {
                foreach(var transaction in AbleTransactions.Where(x => x.CharInserted == letter && x.StartingPoint.Id == this.Id).ToList())
                {
                    possibleStates.Add(transaction.EndingPoint);
                }
                return possibleStates;
            }
            else
            {
                //possibleStates.Add(this);
                return possibleStates;
            }
            
        }
    }
}
