using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    public class Specialist
    {
        public string SpecialistName { get { return FirstName + " " + LastName; } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        public Specialist(string firstName, string lastName, string userName)
        {
            FirstName = firstName;
            LastName = lastName;
            Username = userName;
        }
    }
}
