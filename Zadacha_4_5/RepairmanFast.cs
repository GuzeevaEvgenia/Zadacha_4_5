using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadacha_4_5 
{
    public class RepairmanFast : Repairman
    {
        public RepairmanFast(TransporterManager transporterManager, string name) : base(transporterManager, name + "_Fast", 1000)
        {
        }
    }
}