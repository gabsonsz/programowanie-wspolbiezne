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
        List<Ball> ballList = new List<Ball>();
        private readonly object ballLock = new();

        public Ball(Data.IBall ball, double tw, double th, double border, List<Ball> otherBalls)
        {
            dataBall = ball;
            TableWidth = tw;
            TableHeight = th;
            TableBorder = border;
            ball.NewPositionNotification += RaisePositionChangeEvent;
            ballList = otherBalls;

        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private
        public double TableWidth { get; }
        public double TableHeight { get; }
        public double TableBorder { get; }

        internal void Stop()
        {
            dataBall.Stop();
        }

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {            
                WallCollision(e);
                BallCollision();
                NewPositionNotification?.Invoke(this, new Position(e.x, e.y));            
        }
        private void WallCollision(Data.IVector position)
        {
            lock (ballLock)
            {
                if (position.x >= TableWidth - 20 - 2 * TableBorder || position.x <= 0)
                {
                    dataBall.Velocity.x = -dataBall.Velocity.x;
                }
                if (position.y >= TableHeight - 20 - 2 * TableBorder || position.y <= 0)
                {
                    dataBall.Velocity.y = -dataBall.Velocity.y;
                }
            }
        }
        private void BallCollision()
        {
            double minDistance = 20;

            foreach (Ball other in ballList)
            {
                lock (ballLock)
                {
                    if (other == this) continue;
                    Data.IVector currentPosition = other.dataBall.Position;
                    Data.IVector currentVelocity = other.dataBall.Velocity;

                    double dx = dataBall.Position.x - currentPosition.x;
                    double dy = dataBall.Position.y - currentPosition.y;

                    double euclideanDistance = Math.Sqrt(dx * dx + dy * dy);                    

                    if (euclideanDistance > 0 && euclideanDistance <= minDistance)
                    {

                        double nx = dx / euclideanDistance;
                        double ny = dy / euclideanDistance;


                        //product of Velocity and normal
                        double v1n = dataBall.Velocity.x * nx + dataBall.Velocity.y * ny;
                        double v2n = currentVelocity.x * nx + currentVelocity.y * ny;                                          
                                               
                        
                        double tx = -ny;
                        double ty = nx;
                        double v1t = dataBall.Velocity.x * tx + dataBall.Velocity.y * ty;
                        double v2t = currentVelocity.x * tx + currentVelocity.y * ty;

                        dataBall.Velocity.x = v2n * nx + v1t * tx;
                        dataBall.Velocity.y = v2n * ny + v1t * ty;
                        other.dataBall.Velocity.x = v1n * nx + v2t * tx;
                        other.dataBall.Velocity.y = v1n * ny + v2t * ty;

                        double overlap = minDistance - euclideanDistance;
                        if (overlap > 0)
                        {
                            double adjust = overlap * 0.5;
                            dataBall.Position.x += nx * adjust;
                            dataBall.Position.y += ny * adjust;
                            currentPosition.x -= nx * adjust;
                            currentPosition.y -= ny * adjust;
                        }
                    }
                }

                #endregion private
            }
        }
    }
}