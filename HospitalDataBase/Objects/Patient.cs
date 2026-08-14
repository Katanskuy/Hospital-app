using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDataBase.Objects
{
    public class Patient
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public int Age { get; set; }
        public string SicknessDesc { get; set; }
    }
}
