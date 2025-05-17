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

        internal void Stop()
        {
            dataBall.Stop();
            barrier.RemoveParticipant();
        }

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            //barrier.SignalAndWait();
            lock (ballLock)
            {
                Collision(e);
                NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
            }
            //barrier.SignalAndWait();


        }
        private void Collision(Data.IVector position)
        {

            if (position.x >= TableWidth - dataBall.Diameter - 2 * TableBorder || position.x <= 0)
            {
                dataBall.Velocity.x = -dataBall.Velocity.x;
            }
            if (position.y >= TableHeight - dataBall.Diameter - 2 * TableBorder || position.y <= 0)
            {
                dataBall.Velocity.y = -dataBall.Velocity.y;
            }

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

                if (euclideanDistance > 0 && euclideanDistance <= minDistance)
                {
                    lock (this)
                    {
                        double nx = dx / euclideanDistance;
                        double ny = dy / euclideanDistance;

                        //Velocity
                        double v1x = dataBall.Velocity.x;
                        double v1y = dataBall.Velocity.y;
                        double v2x = other.dataBall.Velocity.x;
                        double v2y = other.dataBall.Velocity.y;

                        //Mass
                        double m1 = dataBall.Mass;
                        double m2 = other.dataBall.Mass;

                        //product of Velocity and normal
                        double v1n = v1x * nx + v1y * ny;
                        double v2n = v2x * nx + v2y * ny;

                        double v1nAfter = (v1n * (m1 - m2) + 2 * m2 * v2n) / (m1 + m2);
                        double v2nAfter = (v2n * (m2 - m1) + 2 * m1 * v1n) / (m1 + m2);

                        double dv1x = (v1nAfter - v1n) * nx;
                        double dv1y = (v1nAfter - v1n) * ny;
                        double dv2x = (v2nAfter - v2n) * nx;
                        double dv2y = (v2nAfter - v2n) * ny;

                        dataBall.Velocity.x += dv1x;
                        dataBall.Velocity.y += dv1y;
                        other.dataBall.Velocity.x += dv2x;
                        other.dataBall.Velocity.y += dv2y;

                        double overlap = minDistance - euclideanDistance;
                        if (overlap > 0)
                        {
                            double adjustX = nx * overlap * 0.5;
                            double adjustY = ny * overlap * 0.5;

                            dataBall.Position.x += adjustX;
                            dataBall.Position.y += adjustY;
                            other.dataBall.Position.x -= adjustX;
                            other.dataBall.Position.y -= adjustY;
                        }
                    }
                }

                #endregion private
            }
        }
    }
}