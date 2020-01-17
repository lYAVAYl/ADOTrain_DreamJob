using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamJob
{
    class Worker
    {
        private string name;
        private double salary;

        public string Name { get; set; }
        public double Salary { get; set; }

        public Worker(string setName = "No Name", double setSalary = 0)
        {
            Name = setName;
            Salary = setSalary;

            name = Name;
            salary = Salary;
        }


        public override string ToString()
        {
            return $"Name [{name}] Salary: [{salary}]";
        }
    }
}
