﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadacha_4_5
{
    public interface IRepairman : IWorker
    {
        int RepairSpeed { get; }
    }
}
