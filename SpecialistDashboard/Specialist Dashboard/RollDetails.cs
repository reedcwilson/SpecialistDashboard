using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    public class RollDetails : Roll
    {
        public string HistoryStep { get; set; }
        public List<Note> Notes { get; set; }
        public List<History> Histories { get; set; }
        public List<ImageNote> ImageNotes { get; set; }

        public RollDetails(int id, string projectId, string rollName, Specialist spec, string state, string step, string historyStep, DateTime lastUpdate)
        {
            //var Data = new DataContext("EPDB01", "JWF_Live");
            //Id = Data.GetWorkItemId(rollName);
            Id = id;
            ProjectId = projectId;
            RollName = rollName;
            Spec = spec;
            State = state;
            Step = step;
            HistoryStep = historyStep;
            LastUpdate = lastUpdate;
        }

        public RollDetails(int id, string projectId, string rollName, Specialist spec, string state, string step, DateTime lastUpdate, List<Note> notes, List<History> histories, List<ImageNote> imageNotes)
        {
            //var Data = new DataContext("EPDB01", "JWF_Live");
            //Id = Data.GetWorkItemId(rollName);
            Id = id;
            ProjectId = projectId;
            RollName = rollName;
            Spec = spec;
            State = state;
            Step = step;
            LastUpdate = lastUpdate;
            Notes = notes;
            Histories = histories;
            ImageNotes = imageNotes;
        }

    }
}
