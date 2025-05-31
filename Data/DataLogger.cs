using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;


namespace TP.ConcurrentProgramming.Data
{
    internal class DataLogger : IDataLogger
    {
        private static readonly Lazy<DataLogger> instance = new Lazy<DataLogger>(() => new DataLogger("../../../../Data/diagnostic_log_file.json"));

        private readonly object fileLock = new object();
        private readonly ConcurrentQueue<BallLog> queue;
        private readonly string filePath;
        private bool isRunning;
        private readonly Thread thread;
        private readonly AutoResetEvent logEvent;
        private const int maxQueueSize = 10000;

        private DataLogger(string path)
        {
            filePath = path;
            logEvent = new AutoResetEvent(false);
            queue = new ConcurrentQueue<BallLog>();

            Debug.WriteLine($"Powstał obiekt i będzie pisał do {filePath}");

            isRunning = true;
            thread = new Thread(movingQueue);
            thread.Start();
        }

        private void movingQueue()
        {
            while (isRunning)
            {
                logEvent.WaitOne();

                while (queue.TryDequeue(out var ballLog))
                {
                    string jsonString = JsonSerializer.Serialize(ballLog);
                    lock (fileLock)
                    {
                        File.AppendAllText(filePath, jsonString + Environment.NewLine);
                    }
                    
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
            isRunning = true;

            if (queue.Count < maxQueueSize)
            {
                var logEntry = new BallLog(position, velocity, DateTime.UtcNow);
                queue.Enqueue(logEntry);
                logEvent.Set();
            }
            else
            {
                Debug.WriteLine("Queue is full");
            }
        }
        private void Stop()
        {
            isRunning = false;
            logEvent.Set();
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

        //public record BallPosition(float X, float Y);
        //public record BallVelocity(float X, float Y);

    }
    
}
