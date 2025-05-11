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
        Data.IBall dataBall;
        //List<Ball> ballList;
        private static object ballLock= new object();

    public Ball(Data.IBall ball, double tw, double th, double border)
        {
            dataBall = ball;
            TableWidth = tw;
            TableHeight = th;
            TableBorder = border;
            ball.NewPositionNotification += RaisePositionChangeEvent;
            //this.ballList = ballList;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private
        public double TableWidth { get; }
        public double TableHeight { get; }
        public double TableBorder { get; }


    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
        lock (ballLock) {
            WallCollision(e);
            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        }
      
    }        
        private void WallCollision(Data.IVector position)
        {            
            double radius = dataBall.Diameter / 2;
            if (position.x  >= TableWidth - dataBall.Diameter - 2 * TableBorder || position.x  <= 0)
            {                
                dataBall.Velocity.x = -dataBall.Velocity.x;
            }
            if (position.y >= TableHeight - dataBall.Diameter - 2 * TableBorder || position.y  <= 0)
            {
                dataBall.Velocity.y = -dataBall.Velocity.y;
            }
            
        }

    #endregion private
  }
}