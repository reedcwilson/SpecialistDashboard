using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Specialist_Dashboard
{
    class QALogReader
    {
        public Roll Roll { get; set; }
        public Dictionary<string, List<string>> Images { get; set; }

        public QALogReader(Roll roll)
        {
            this.Roll = roll;
            Images = new Dictionary<string, List<string>>();
        }

        public Dictionary<string, List<string>> GetImages()
        {
            var rollPaths = new RollPaths(this.Roll);
            string qALog = rollPaths.GetImgProcessPath() + @"\ImageQA.log";
            if (File.Exists(qALog))
            {
                string text = File.ReadAllText(qALog);

                var matches = Regex.Matches(text, @"(_|-)(\d{5})\..*=(\w+)")
                    .Cast<Match>();

                foreach (var match in matches)
                {
                    var key = match.Groups[3].Value;
                    var number = int.Parse(match.Groups[2].Value);
                    if (Images.ContainsKey(key))
                        Images[key].Add(number.ToString());
                    else
                        Images.Add(key, new List<string> { number.ToString() });
                }
                return Images;
            }
            else return null;
        }
    }
}
