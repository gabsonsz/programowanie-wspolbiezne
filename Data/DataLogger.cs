using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Concurrent;

namespace TP.ConcurrentProgramming.Data
{
    class DataLogger
    {
        private static readonly Lazy<DataLogger> instance = new(() => new DataLogger("../Log/diagnostic_log_file.json"));
        public static DataLogger Instance => instance.Value;
        private readonly ConcurrentQueue<BallLog> queue = new();
        private readonly string filePath;
        private bool isRunning = true;
        private readonly Thread thread;
        private const int maxQueueSize = 10000;
        
            private DataLogger(string path)
        {
            filePath = path;
            thread = new Thread(movingQueue);
            thread.Start();
        }

        private void movingQueue()
        {
            while (isRunning)
            {

            }
        }
        private void Stop() { 
         isRunning = false;
        }

    }
    class BallLog
    {
        private readonly IVector Position ;
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
