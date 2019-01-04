using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Zadacha_4_5 
{
    public class Transporter : IWorker
    {
        public List<Product> Products = new List<Product>();

        public string Name { get; }
        public Vector2 Pos { get; }

        private bool transporterWorking = false;

        DateTime newTransporter; 
        int newTransporterTimeSpan = 10;

        object transporterUpdate = new object();
        TransporterManager transporterManager;

        int minProductCount = 3;

        public Transporter(TransporterManager transporterManager, Vector2 pos, string name)
        {
            Pos = pos;
            Name = "Transporter_" + name;
            this.transporterManager = transporterManager;
        }

        public void Start()
        {
            transporterWorking = true;
            newTransporter = DateTime.UtcNow.AddSeconds(newTransporterTimeSpan);
        }

        public void Update()
        {
            while (true)
            {
                if (transporterWorking)
                {
                    Move();
                    CheckForLoad();
                    transporterManager.OnProgress(this);
                    CheckCrash();
                }

                Thread.Sleep(100);
            }
        }

        public void LoadTransporter(Product product)
        {
            lock (transporterUpdate)
            {
                product.ReachEnd += ReachEnd;
                transporterManager.NewProduct(product);
                Products.Add(product);
            }
        }

        private void Move()
        {
            for (int i = 0; i < Products.Count; i++)
            {
                var product = Products[i];
                product.Move();
            }
        }

        private void ReachEnd(Product product)
        {
            lock (transporterUpdate)
            {
                if (product != null)
                {
                    var message =
                        $"Products ReachEnd: {product.GetHashCode()} count: {Products.Count} transporterName: {Name}";
                    Debug.Print(message);
                    transporterManager.AddToStore(product);
                    Products.Remove(product);
                    product.ReachEnd -= ReachEnd;
                }
                else
                {
                    var message = $"first == null ProductsCount: {Products.Count} transporterName: {Name}";
                    Debug.Print(message);
                }
            }
        }


        bool loadingWorking = false;
        private void CheckForLoad()
        {
            if (Products.Count <= minProductCount && !loadingWorking)
            {
                loadingWorking = true;
                transporterManager.AddForLoading(this);
            }
        }

        private void CheckCrash()
        {
            if (DateTime.UtcNow > newTransporter)
            {
                var randomCrashChance = new Random();
                var crashChance = randomCrashChance.Next(11);
                if (crashChance == 10 && transporterWorking)
                {
                    Debug.Print($"!!!!!!!!!!!! Transporter Broke: {Name}");
                    Stop();
                    transporterManager.AddForRepair(this);
                }
            }
        }

        public void Loaded()
        {
            loadingWorking = false;
        }

        public void Stop()
        {
            transporterWorking = false;
            loadingWorking = false;
        }
    }
}

