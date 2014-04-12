namespace Physicist.Actors
{
    using System;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Dynamics.Contacts;

    public class CollisionSensor
    {
        public CollisionSensor(Fixture sensorFixture, string sensorName)
        {
            this.SensorName = sensorName;
            sensorFixture.IsSensor = true;
            sensorFixture.OnCollision += this.SensorBody_OnCollision;
            sensorFixture.OnSeparation += this.SensorBody_OnSeparation;
        }

        public event OnCollisionEventHandler CollisionDetected;

        public event OnSeparationEventHandler SeparationDetected;

        public string SensorName
        {
            get;
            private set;
        }

        private bool SensorBody_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (this.CollisionDetected != null)
            {
                this.CollisionDetected(fixtureA, fixtureB, contact);
            }

            return true;
        }

        private void SensorBody_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (this.SeparationDetected != null)
            {
                this.SeparationDetected(fixtureA, fixtureB);
            }
        }
    }
}
