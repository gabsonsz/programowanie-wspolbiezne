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

        #endregion IBall

        #region private
        private Thread ballThread;
        private bool isRunning = true;

        private void threadLoop()
        {
            while (isRunning)
            {
                Move();
                Thread.Sleep(40);
            }
        }

        public void Dispose()
        {
            isRunning = false;
            ballThread?.Join();
        }
    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    private void Move()
    {
      Position = new Vector(Position.x + Velocity.x, Position.y + Velocity.y);
      RaiseNewPositionChangeNotification();
    }

    #endregion private
  }
}