﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Numerics;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BallUnitTest
  {
        [TestMethod]
        public void MoveTestMethod()
        {            
            List<Ball> balls = new List<Ball>();
            DataBallFixture dataBallFixture = new DataBallFixture(new VectorFixture(0,0), new VectorFixture(0,0));
            Ball newInstance = new(dataBallFixture, 400, 400, 50, balls);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
            dataBallFixture.Move();
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
        }

        #region testing instrumentation

        private class DataBallFixture : Data.IBall
    {
            public DataBallFixture(Data.IVector Velocity, Data.IVector Position) {
                this.Velocity = Velocity;
                this.Position = Position;
            }

            public double Diameter { get; } = 20;
            public double Mass { get;} = 100;
      public Data.IVector Position { get;  }
      public Data.IVector Velocity { get; set; }

      public event EventHandler<Data.IVector>? NewPositionNotification;

      public void Stop()
        {

        }

      internal void Move()
      {
        NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
      }
    }

    private class VectorFixture : Data.IVector
    {
      internal VectorFixture(double X, double Y)
      {
        x = X; y = Y;
      }

      public double x { get; set; }
      public double y { get; set; }
    }

    #endregion testing instrumentation
  }
}