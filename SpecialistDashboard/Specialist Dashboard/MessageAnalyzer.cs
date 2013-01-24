using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class MessageAnalyzer
    {
        public List<ImageFileNote> Chops { get; set; }
        public List<ImageFileNote> Lights { get; set; }
        public List<ImageFileNote> Darks { get; set; }
        public List<ImageFileNote> Others { get; set; }
        public List<ImageFileNote> Stretches { get; set; }
        public List<ImageFileNote> Bandings { get; set; }
        public List<ImageFileNote> Missings { get; set; }

        public MessageAnalyzer()
        {
            Others = new List<ImageFileNote>();
            Missings = new List<ImageFileNote>();
            Lights = new List<ImageFileNote>();
            Darks = new List<ImageFileNote>();
            Stretches = new List<ImageFileNote>();
            Chops = new List<ImageFileNote>();
            Bandings = new List<ImageFileNote>();
        }

        public Dictionary<string, List<ImageFileNote>> Analyze(string rollName)
        {
            var notesReader = new NoteFileReader();
            var notes = notesReader.GetNotes(rollName);

            foreach (var note in notes)
            {
                if (note.NoteMessage.ToLower() == "automated message - image marked as other error by auditor")
                    Others.Add(note);
                else if (note.NoteMessage.ToLower() == "automated message - image marked as missing error by auditor")
                    Missings.Add(note);
                else if (note.NoteMessage.ToLower() == "automated message - image marked as light error by auditor")
                    Lights.Add(note);
                else if (note.NoteMessage.ToLower() == "automated message - image marked as dark error by auditor")
                    Darks.Add(note);
                else if (note.NoteMessage.ToLower() == "automated message - image marked as stretched error by auditor")
                    Stretches.Add(note);
                else if (note.NoteMessage.ToLower() == "automated message - image marked as banding error by auditor")
                    Bandings.Add(note);
                else if (note.NoteMessage.ToLower() == "automated message - image marked as chopped error by auditor")
                    Chops.Add(note);
            }
            var categories = new Dictionary<string, List<ImageFileNote>>();
            categories.Add("Other", Others);
            categories.Add("Chop", Chops);
            categories.Add("Missing", Missings);
            categories.Add("Light", Lights);
            categories.Add("Dark", Darks);
            categories.Add("Banding", Bandings);
            categories.Add("Stretch", Stretches);

            return categories;
        }
    }
}
