//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall
  {
    public Ball(Data.IBall ball, double tw,double th)
    {
            TableWidth = tw;
            TableHeight = th;
      ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private
        public double TableWidth { get; }
        public double TableHeight { get; }


    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

    private void WallCollision(Data.IBall ball)
        {
            //wall collision
            double radius = ball.Diameter/2;
            if (ball.Position.x + radius >= TableWidth || ball.Position.x - radius <= 0)
            {                
                ball.Velocity.x = -ball.Velocity.x;
            }
            if (ball.Position.y + radius >= TableHeight || ball.Position.y - radius <= 0)
            {
                ball.Velocity.y = -ball.Velocity.y;
            }
            
        }
    #endregion private
  }
}