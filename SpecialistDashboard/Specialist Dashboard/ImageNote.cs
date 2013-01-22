using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    public class ImageNote : IComparable<ImageNote>
    {
        public Specialist Spec { get; set; }
        public string NoteMessage { get; set; }
        public DateTime UpdateTime { get; set; }
        public int ImageNum { get; set; }

        public ImageNote(Specialist spec, string noteMessage, DateTime updateTime, int imageNum)
        {
            Spec = spec;
            NoteMessage = noteMessage;
            UpdateTime = updateTime;
            ImageNum = imageNum;
        }

        public int CompareTo(ImageNote b)
        {
            // Integer sort
            return this.ImageNum.CompareTo(b.ImageNum);
        }
    }
}
