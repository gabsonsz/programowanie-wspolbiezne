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
                if (position.x >= TableWidth - dataBall.Diameter - 2 * TableBorder || position.x <= 0)
                {
                    dataBall.Velocity.x = -dataBall.Velocity.x;
                }
                if (position.y >= TableHeight - dataBall.Diameter - 2 * TableBorder || position.y <= 0)
                {
                    dataBall.Velocity.y = -dataBall.Velocity.y;
                }
            }
        }
        private void BallCollision()
        {

            foreach (Ball other in ballList)
            {
                lock (ballLock)
                {
                    if (other == this) continue;

                    double dx = dataBall.Position.x - other.dataBall.Position.x;
                    double dy = dataBall.Position.y - other.dataBall.Position.y;

                    double euclideanDistance = Math.Sqrt(dx * dx + dy * dy);
                    double minDistance = (dataBall.Diameter + other.dataBall.Diameter) / 2;

                    if (euclideanDistance > 0 && euclideanDistance <= minDistance)
                    {

                        double nx = dx / euclideanDistance;
                        double ny = dy / euclideanDistance;


                        //product of Velocity and normal
                        double v1n = dataBall.Velocity.x * nx + dataBall.Velocity.y * ny;
                        double v2n = other.dataBall.Velocity.x * nx + other.dataBall.Velocity.y * ny;

                        //Mass
                        double m1 = dataBall.Mass;
                        double m2 = other.dataBall.Mass;

                        double v1nAfter = (v1n * (m1 - m2) + 2 * m2 * v2n) / (m1 + m2);
                        double v2nAfter = (v2n * (m2 - m1) + 2 * m1 * v1n) / (m1 + m2);

                        double tx = -ny;
                        double ty = nx;
                        double v1t = dataBall.Velocity.x * tx + dataBall.Velocity.y * ty;
                        double v2t = other.dataBall.Velocity.x * tx + other.dataBall.Velocity.y * ty;

                        dataBall.Velocity.x = v1nAfter * nx + v1t * tx;
                        dataBall.Velocity.y = v1nAfter * ny + v1t * ty;
                        other.dataBall.Velocity.x = v2nAfter * nx + v2t * tx;
                        other.dataBall.Velocity.y = v2nAfter * ny + v2t * ty;

                        double overlap = minDistance - euclideanDistance;
                        if (overlap > 0)
                        {
                            double adjust = overlap * 0.5;
                            dataBall.Position.x += nx * adjust;
                            dataBall.Position.y += ny * adjust;
                            other.dataBall.Position.x -= nx * adjust;
                            other.dataBall.Position.y -= ny * adjust;
                        }
                    }
                }

                #endregion private
            }
        }
    }
}