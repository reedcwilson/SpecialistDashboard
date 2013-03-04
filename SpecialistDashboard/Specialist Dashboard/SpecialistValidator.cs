using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specialist_Dashboard
{
    class SpecialistValidator
    {
        public static string[] QEAuditors = new string[]
        {
            "miranda.milligan", 
            "samuel.butler", 
            "reed.wilson", 
            "david.crockett",
            "andrew.thomas",
            "scott.spencer",
            "christpher.felt",
            "mike.gillotti",
            "tamra.ockey",
            "ryan.smith"
        };

        public static string[] Supervisors = new string[]
        {
            "miranda.milligan", 
            "samuel.butler", 
            "reed.wilson", 
            "david.crockett"
        };

        public static bool ValidateAuditor()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            foreach (var auditor in QEAuditors)
            {
                if (auditor == userName)
                    return true;
            }
            return false;
        }

        public static bool ValidateSupervisor()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            foreach (var specialist in Supervisors)
            {
                if (specialist == userName)
                    return true;
            }
            return false;
        }
    }
}
