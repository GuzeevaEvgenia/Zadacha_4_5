using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Zadacha_4_5
{
    public class Repairman : IRepairman
    {
        public string Name { get; }

        public int RepairSpeed { get; }

        DateTime repairUpdateTime;
        TransporterManager transporterManager;
        public Repairman(TransporterManager transporterManager, string name, int repairTime)
        {
            Name = "Repairman_" + name;
            RepairSpeed = repairTime;
            this.transporterManager = transporterManager;
        }

        public void Start()
        {
            repairUpdateTime = DateTime.UtcNow.AddMilliseconds(RepairSpeed);
        }
        Transporter transporter = null;
        public void Update()
        {
            while (true)
            {
                if (transporterManager.CheckRepairQueue() && transporter == null)
                {
                    transporter = transporterManager.GetToRepairWorker() as Transporter;
                    if (transporter == null)
                        continue;
                    repairUpdateTime = DateTime.UtcNow.AddMilliseconds(RepairSpeed);

                    Debug.WriteLine($"------------ Repairing BROKE repairName: {Name} transporter: {transporter.Name} Time: {DateTime.UtcNow}");
                }

                if (transporter != null)
                {
                    if (DateTime.UtcNow > repairUpdateTime)
                    {
                        transporter.Start();

                        var message = $"------------ Repairing START repairName: {Name} transporter: {transporter.Name} Time: {DateTime.UtcNow}";
                        Debug.WriteLine(message);

                        transporter = null;
                        repairUpdateTime = DateTime.MaxValue;
                    }
                }

                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            repairUpdateTime = DateTime.MaxValue;
        }
    }
}
