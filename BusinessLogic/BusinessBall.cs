//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Numerics;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        Data.IBall dataBall;
        List<Ball> ballList = new List<Ball>();
        private Barrier barrier;
        private static object ballLock = new object();

        public Ball(Data.IBall ball, double tw, double th, double border, List<Ball> otherBalls, Barrier barrier)
        {
            dataBall = ball;
            TableWidth = tw;
            TableHeight = th;
            TableBorder = border;
            ball.NewPositionNotification += RaisePositionChangeEvent;
            ballList = otherBalls;
            this.barrier = barrier;

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
            lock (ballLock)
            {
                WallCollision(e);
                BallCollision();
                NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
            }

        }
        private void WallCollision(Data.IVector position)
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

        private void BallCollision()
        {
            foreach (Ball other in ballList)
            {
                if (other == this) continue;

                double x1 = dataBall.Position.x;
                double y1 = dataBall.Position.y;

                double x2 = other.dataBall.Position.x;
                double y2 = other.dataBall.Position.y;

                double dx = x1 - x2;
                double dy = y1 - y2;

                double euclideanDistance = Math.Sqrt(dx * dx + dy * dy);
                double minDistance = (dataBall.Diameter + other.dataBall.Diameter) / 2;

                if (euclideanDistance > 0 && euclideanDistance < minDistance)
                {

                    double nx = dx / euclideanDistance;
                    double ny = dy / euclideanDistance;

                    double v1x = dataBall.Velocity.x;
                    double v1y = dataBall.Velocity.y;

                    double v2x = other.dataBall.Velocity.x;
                    double v2y = other.dataBall.Velocity.y;

                    double v1n = v1x * nx + v1y * ny;  
                    double v2n = v2x * nx + v2y * ny;  

                    double v1nAfter = v2n;
                    double v2nAfter = v1n;

                    double dv1x = (v1nAfter - v1n) * nx;
                    double dv1y = (v1nAfter - v1n) * ny;
                    double dv2x = (v2nAfter - v2n) * nx;
                    double dv2y = (v2nAfter - v2n) * ny;

                    dataBall.Velocity.x += dv1x;
                    dataBall.Velocity.y += dv1y;
                    other.dataBall.Velocity.x += dv2x;
                    other.dataBall.Velocity.y += dv2y;

                    double overlap = 20 - euclideanDistance;  
                    if (overlap > 0)
                    {
                        double adjustX = nx * overlap * 0.5;
                        double adjustY = ny * overlap * 0.5;

                        dataBall.Position.x += adjustX;
                        dataBall.Position.y += adjustY;
                        other.dataBall.Position.x -= adjustX;
                        other.dataBall.Position.y -= adjustY;
                    }

                    //double v1_x = dataBall.Velocity.x;
                    //double v1_y = dataBall.Velocity.y;

                    //double v2_x = other.dataBall.Velocity.x;
                    //double v2_y = other.dataBall.Velocity.y;

                    //double m1 = dataBall.Mass;
                    //double m2 = other.dataBall.Mass;

                    ////collision normal
                    //double nx = dx / euclideanDistance;
                    //double ny = dy / euclideanDistance;

                    ////velocity vector
                    //double dv_x = v1_x - v2_x;
                    //double dv_y = v1_y - v2_y;

                    //double adjust = dv_x * nx + dv_y * ny;

                    ////Impulse
                    //double impulse = (2 * adjust) / (m1 + m2);

                    ////new velocity values
                    //double v1x = v1_x - impulse * m2 * nx;
                    //double v1y = v1_y - impulse * m2 * ny;

                    //double v2x = v2_x - impulse * m1 * nx;
                    //double v2y = v2_y - impulse * m1 * ny;


                    //dataBall.Velocity.x = v1x;
                    //dataBall.Velocity.y = v1y;

                    //other.dataBall.Velocity.x = v2x;
                    //other.dataBall.Velocity.y = v2y;

                }


            }
        }

        #endregion private
    }
}