using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    public class Roll : IComparable<Roll>
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string RollName { get; set; }
        public int Priority { get; set; }
        public Specialist Spec { get; set; }
        public string State { get; set; }
        public string Step { get; set; }
        public DateTime LastUpdate { get; set; }

        //public string UserName { get; set; }

        public Roll()
        {
        }

        public Roll(int id, string projectId, string rollName, int priority, Specialist spec, string state, string step)
        {
            //var Data = new DataContext("EPDB01", "JWF_Live");
            //Id = Data.GetWorkItemId(rollName);
            Id = id;
            ProjectId = projectId;
            RollName = rollName;
            Priority = priority;
            Spec = spec;
            State = state;
            Step = step;


        }


        public Roll(int id, string projectId, string rollName, int priority, string userName, string state, string step, DateTime lastUpdate)
        {
            //var Data = new DataContext("EPDB01", "JWF_Live");
            //Id = Data.GetWorkItemId(rollName);
            Id = id;
            ProjectId = projectId;
            RollName = rollName;
            Priority = priority;
            Spec = new Specialist(null, null, userName.Substring(9));
            State = state;
            Step = step;
            LastUpdate = lastUpdate;
        }

        public int CompareTo(Roll b)
        {
            // Alphabetic sort name[A to Z]
            return this.RollName.CompareTo(b.RollName);
        }
    }
}
