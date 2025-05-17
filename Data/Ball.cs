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
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity, double mass, double diameter)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
      Mass = mass;
      Diameter = diameter;
      refreshTime = 20;
      ThreadStart ts = new ThreadStart(threadLoop);
      ballThread = new System.Threading.Thread(ts);
      ballThread.Start();
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }
    public IVector Position { get; set; }

    public double Mass { get; }
    public double Diameter { get;  }
        private int refreshTime;
        #endregion IBall

        #region private
        private Thread ballThread;
        private bool isRunning = true;
        

        private void threadLoop()
        {
            while (isRunning)
            {
                Move();
                Thread.Sleep(refreshTime);
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
        private void ChangeRefreshTime()
        {
            double accualVelocity = Math.Sqrt(Velocity.x * Velocity.x + Velocity.y * Velocity.y);
            int maxRefreshTime = 100;
            int minRefreshTime = 10;

            double normalizedVelocity= Math.Clamp(accualVelocity, 0.0, 1.0);
            refreshTime = (int)(maxRefreshTime - normalizedVelocity * (maxRefreshTime - minRefreshTime));
        }
        internal void Move()
    {
        ChangeRefreshTime();
      Position = new Vector(Position.x + (Velocity.x*refreshTime/1000), Position.y + (Velocity.y*refreshTime/1000));
      RaiseNewPositionChangeNotification();
    }

    #endregion private
  }
}