using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSelfDeclarationLib.OraDatabase.Models
{
    public class PersonInfo
    {
        public string Initials { get; set; }
        public string Address { get; set; }
        public string BirthLocation { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
