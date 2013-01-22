using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class GetRollsArgument
    {
        public string Step { get; set; }
        public string State { get; set; }
        public string Priority { get; set; }
        public string Project { get; set; }
        public string Roll { get; set; }
        public Specialist Specialist { get; set; }

        public GetRollsArgument(string step, string state, string priority, string project, string roll, Specialist spec)
        {
            this.Step = step;
            this.State = state;
            this.Priority = priority;
            this.Project = project;
            this.Roll = roll;
            this.Specialist = spec;
        }
    }
}
