using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Specialist_Dashboard
{
    class SpecialistValidator
    {
        private static List<string> ValidAuditors;
        public static List<string> GetValidAuditors()
        {
            ValidAuditors = ReadValids(@"\\dpfs01\dpsfiler\Imaging\Production_Team\Supervisors\SpecDashValidator\ValidAuditors.txt");
            return ValidAuditors;
        }

        private static List<string> ValidSupers;
        public static List<string> GetValidSupers()
        {
            ValidSupers = ReadValids(@"\\dpfs01\dpsfiler\Imaging\Production_Team\Supervisors\SpecDashValidator\ValidSupers.txt");
            return ValidSupers;
        }

        //public static string[] QEAuditors = new string[]
        //{
        //    "miranda.milligan", 
        //    "samuel.butler", 
        //    "reed.wilson", 
        //    "david.crockett",
        //    "andrew.thomas",
        //    "scott.spencer",
        //    "christpher.felt",
        //    "mike.gillotti",
        //    "tamra.ockey",
        //    "ryan.smith"
        //};

        //public static string[] Supervisors = new string[]
        //{
        //    "miranda.milligan", 
        //    "samuel.butler", 
        //    "reed.wilson", 
        //    "david.crockett"
        //};

        public static bool ValidateAuditor()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            foreach (var auditor in GetValidAuditors())
            {
                if (@"myfamily\" + auditor == userName.ToLower())
                    return true;
            }
            return false;
        }

        public static bool ValidateSupervisor()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            foreach (var specialist in GetValidSupers())
            {
                if (@"myfamily\" + specialist == userName.ToLower())
                    return true;
            }
            return false;
        }

        private static List<string> ReadValids(string path)
        {
            var valids = new List<string>();
            string line;

            using (var sr = new StreamReader(path))
            {
                while ((line = sr.ReadLine()) != null)
                    if (line.Trim() != "")
                        valids.Add(line.Trim());
            }
            return valids;
        }


    }
}
