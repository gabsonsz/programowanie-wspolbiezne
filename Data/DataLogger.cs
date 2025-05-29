using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace TP.ConcurrentProgramming.Data
{
    class DataLogger
    {
        private static readonly Lazy<DataLogger> instance = new(() => new DataLogger("../Log/diagnostic_log_file.json"));
        public static DataLogger Instance => instance.Value;
        private readonly ConcurrentQueue<BallLog> queue;
        private readonly string filePath;
        private bool isRunning;
        private readonly Thread thread;
        private AutoResetEvent logEvent;
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
                    File.AppendAllText(filePath, jsonString+ Environment.NewLine);
                }
            }
        }
        private void Stop()
        {
            isRunning = false;
        }

    }
    class BallLog
    {
        private readonly IVector Position;
        private readonly IVector Velocity;
        private readonly DateTime Time;
        public BallLog(IVector position, IVector velocity, DateTime time)
        {
            Position = position;
            Velocity = velocity;
            Time = time;

        }
    }
}
