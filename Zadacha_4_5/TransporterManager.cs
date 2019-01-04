using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Zadacha_4_5
{
    public class TransporterManager
    {
        private Thread[] m_workingThreads;

        public List<IWorker> Workers = new List<IWorker>();

        ConcurrentQueue<IWorker> concurrentQueueToRepair = new ConcurrentQueue<IWorker>();
        ConcurrentQueue<IWorker> concurrentQueueToLoad = new ConcurrentQueue<IWorker>();

        public Action<Transporter> Progress;
        public Action<Transporter> Draw;
        public Action<Transporter> Broke;
        public Action<Product> DrawNewProduct;
        public Action<Product> AddToStoreProduct;

        public void InitTransporters(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var pos = new Vector2(200 + 40 * i, 300);
                var transporter = new Transporter(this, pos, i.ToString());
                Workers.Add(transporter);
                transporter.Start();
                OnDraw(transporter);
            }
        }
        public void InitLoaders(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var loader = new Loader(this, i.ToString());
                Broke += loader.StopLoad;
                Workers.Add(loader);
                loader.Start();
            }
        }

        public void InitRepairs(int count)
        {
            var repairmenFast = new RepairmanFast(this, "0");
            Workers.Add(repairmenFast);
            repairmenFast.Start();

            var repairmenLazy = new RepairmanLazy(this, "1");
            Workers.Add(repairmenLazy);
            repairmenLazy.Start();
        }
        public void StartManager()
        {
            m_workingThreads = new Thread[Workers.Count];
            for (int i = 0; i < Workers.Count; i++)
            {
                var worker = Workers[i];
                m_workingThreads[i] = new Thread(worker.Update);
                m_workingThreads[i].Name = worker.Name + "_thread" + i;
                m_workingThreads[i].Start();
            }
        }

        public void NewProduct(Product product)
        {
            DrawNewProduct?.Invoke(product);
        }

        public void OnProgress(Transporter transporter)
        {
            Progress?.Invoke(transporter);
        }

        public void OnDraw(Transporter transporter)
        {
            Draw?.Invoke(transporter);
        }

        public bool working = true;
        List<Product> storeList = new List<Product>();
        object storeObj = new object();
        public void AddToStore(Product product)
        {
            lock (storeObj)
            {
                storeList.Add(product);
                AddToStoreProduct?.Invoke(product);
                var message = $"Store count: {storeList.Count}";
                Debug.Print(message);
            }
        }

        public void AddForLoading(IWorker worker)
        {
            concurrentQueueToLoad.Enqueue(worker);

            Debug.Print($"AddForLoading count: {concurrentQueueToLoad.Count}");
        }

        public bool CheckLoadingQueue()
        {
            return concurrentQueueToLoad.Count > 0;
        }

        public IWorker GetToLoadingWorker()
        {
            IWorker worker = null;
            if (concurrentQueueToLoad.TryDequeue(out worker))
            {
                Debug.Print($"GetToLoadingWorker count: {concurrentQueueToLoad.Count}");

                return worker;
            }

            Debug.Print($"ERROR GetToLoadingWorker worker == null count: {concurrentQueueToLoad.Count}");
            return worker;
        }

        public void AddForRepair(IWorker worker)
        {
            if (worker is Transporter)
            {
                var transporter = worker as Transporter;
                Broke?.Invoke(transporter);
            }

            concurrentQueueToRepair.Enqueue(worker);
            Debug.Print($"AddForRepair count: {concurrentQueueToRepair.Count}");
        }

        public bool CheckRepairQueue()
        {
            return concurrentQueueToRepair.Count > 0;
        }

        public IWorker GetToRepairWorker()
        {
            IWorker worker;
            if (concurrentQueueToRepair.TryDequeue(out worker))
            {
                Debug.Print($"GetToRepairWorker count: {concurrentQueueToRepair.Count}");
                return worker;
            }
            return worker;
        }

        public void StartTrasporters()
        {
            var transporters = Workers.FindAll(w => w is Transporter);
            foreach (var transporter in transporters)
            {
                transporter.Start();
            }

            working = true;
        }

        public void StopTrasporters()
        {
            var transporters = Workers.FindAll(w => w is Transporter);
            foreach (var transporter in transporters)
            {
                transporter.Stop();
            }

            working = false;
        }
    }
}
