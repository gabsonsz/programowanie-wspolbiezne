using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataLogger : IDataLogger
    {
        private static readonly Lazy<DataLogger> instance = new Lazy<DataLogger>(() => new DataLogger("../../../../DataLog/diagnostic_log_file.json"));

        private readonly ConcurrentQueue<BallLog> queue;
        private readonly SemaphoreSlim semaphore;
        private readonly string filePath;
        private bool isRunning;
        private readonly Thread thread;
        private readonly AutoResetEvent logEvent;
        private const int maxQueueSize = 10000;

        private DataLogger(string path)
        {
            semaphore = new SemaphoreSlim(maxQueueSize);
            filePath = path;
            logEvent = new AutoResetEvent(false);
            queue = new ConcurrentQueue<BallLog>();

            isRunning = true;
            thread = new Thread(movingQueue);
            thread.Start();
        }

        private void movingQueue()
        {
            while (isRunning || !queue.IsEmpty)
            {
                logEvent.WaitOne();

                while (queue.TryDequeue(out var ballLog))
                {
                    string jsonString = JsonSerializer.Serialize(ballLog);
                
                    File.AppendAllText(filePath, jsonString + Environment.NewLine);

                    semaphore.Release();
                }
            }
        }

        public static DataLogger LoggerInstance
        {
            get
            {
                return instance.Value;
            }
        }

        public void Log(IVector position, IVector velocity)
        {
            if (!isRunning) return;

            if (semaphore.Wait(0))
            {
                queue.Enqueue(new BallLog(position, velocity, DateTime.UtcNow));
                logEvent.Set();
            }
            else
            {
                Debug.WriteLine("Queue is full");
            }
        }
        public void Stop()
        {
            if (!isRunning) return;
            isRunning = false;
            logEvent.Set();
            thread.Join();
        }

        internal class BallLog
        {
            public IVector Position { get; set; }
            public IVector Velocity { get; set; }
            public DateTime Time { get; set;  }
            public BallLog(IVector position, IVector velocity, DateTime time)
            {
                Position = position;
                Velocity = velocity;
                Time = time;

            }
        }

    }
    
}
