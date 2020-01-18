using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamJob
{
    class Staff
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public Staff(int id, string name, bool active)
        {
            ID = id; 
            Name = name; 
            IsActive = active;
        }


        public override string ToString()
        {
            return $"[{ID}] [{Name}] [{IsActive}]";
        }
    }
}
