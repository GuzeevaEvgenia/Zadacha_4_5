using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Zadacha_4_5
{
    public class Loader : IWorker
    {
        public string Name { get; }

        DateTime loadUpdateTime;
        int loadSpeed = 1000;

        int maxProductCount = 10;
        TransporterManager transporterManager;
        public Loader(TransporterManager transporterManager, string name)
        {
            Name = "Loader_" + name;
            loadUpdateTime = DateTime.MaxValue;
            this.transporterManager = transporterManager;
        }

        public void Start()
        {
            loadUpdateTime = DateTime.UtcNow.AddMilliseconds(loadSpeed);
        }

        Transporter transporter = null;
        public void Update()
        {
            while (true)
            {
                if (transporterManager.CheckLoadingQueue() && transporter == null)
                {
                    transporter = transporterManager.GetToLoadingWorker() as Transporter;
                    if (transporter == null)
                        continue;
                    loadUpdateTime = DateTime.UtcNow.AddMilliseconds(loadSpeed);

                    Debug.WriteLine($".......loading loaderName: {Name} transporter: {transporter.Name} Time: {DateTime.UtcNow}");
                }

                if (transporter != null)
                {
                    AddNewProduct();
                    CheckEnouthProduct();
                }

                Thread.Sleep(100);
            }
        }

        void AddNewProduct()
        {
            if (DateTime.UtcNow >= loadUpdateTime)
            {
                var product = new Product(transporter.Pos);
                transporter.LoadTransporter(product);
                loadUpdateTime = DateTime.UtcNow.AddMilliseconds(loadSpeed);
            }
        }

        void CheckEnouthProduct()
        {
            if (transporter.Products.Count >= maxProductCount)
            {
                transporter.Loaded();
                transporter = null;
            }
        }

        public void StopLoad(Transporter t)
        {
            if (transporter == t)
            {
                transporter = null;
            }
        }

        public void Stop()
        {
            loadUpdateTime = DateTime.MaxValue;
        }
    }
}