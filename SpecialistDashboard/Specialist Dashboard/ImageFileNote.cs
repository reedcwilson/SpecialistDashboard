using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    public class ImageFileNote
    {
        public string User { get; set; }
        public string NoteMessage { get; set; }
        public int ImageNum { get; set; }

        public ImageFileNote(string user, string noteMessage, int imageNum)
        {
            User = user;
            NoteMessage = noteMessage;
            ImageNum = imageNum;
        }
    }
}
