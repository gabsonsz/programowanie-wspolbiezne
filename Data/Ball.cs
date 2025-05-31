//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        private Vector position;
        private Vector velocity;
        private readonly object positionLock = new();
        private readonly object velocityLock = new();
        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            position = initialPosition;
            velocity = initialVelocity;
            
        }

        public void Start()
        {
            ThreadStart ts = new ThreadStart(threadLoop);
            ballThread = new System.Threading.Thread(ts);
            ballThread.Start();
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity
        {
            get
            {
                lock (velocityLock)
                {
                    return velocity;
                }
            }
            set
            {
                lock (velocityLock)
                {
                    velocity = (Vector)value;
                }

            }
        }
        public IVector Position
        {
            get
            {
                lock (positionLock)
                {
                    return position;
                }
            }
        }

        #endregion IBall

        #region private
        private Thread ballThread;
        private bool isRunning = true;
        private DateTime lastUpdateTime = DateTime.UtcNow;

        private void threadLoop()
        {
            while (isRunning)
            {
                
                Move();
                
                Thread.Sleep(5);
            }
        }

        public void Stop()
        {
            isRunning = false;
        }

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        private void Move()
        {
            DateTime now = DateTime.UtcNow;
            double deltaTime = (now - lastUpdateTime).TotalSeconds;
            lastUpdateTime = now;

            lock (positionLock)
            {
                position = new Vector(
                    position.x + (Velocity.x * deltaTime),
                    position.y + (Velocity.y * deltaTime)
                );
            }
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}