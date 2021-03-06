﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamJob
{
    class Worker
    {
        public string Name { get; set; }
        public double Salary { get; set; }

        public Worker(string setName = "No Name", double setSalary = 0)
        {
            Name = setName;
            Salary = setSalary;
        }


        public override string ToString()
        {
            return $"[{Name}] [{Salary}]";
        }
    }
}
