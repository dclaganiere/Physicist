namespace Physicist.Actors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using FarseerPhysics;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Dynamics.Joints;
    using FarseerPhysics.Factories;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Physicist.Controls;
    using Physicist.Enums;
    using Physicist.Extensions;

    public class Player : Actor, IXmlSerializable
    {
        private bool onGround = true;

        public Player(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            this.XmlDeserialize(element);
        }

        public Player() : 
            base()
        {
        }

        public new Body Body
        {
            get
            {
                return base.Body;
            }

            set
            {
                base.Body = value;
                base.Body.OnCollision += this.Body_OnCollision;

                // *************************DIMENSIONS ARE TEST VALUES*********************************** //
                CollisionSensor jumpSensor = new CollisionSensor(FixtureFactory.AttachRectangle(16, 3, 1f, new Vector2(0, 19), this.Body), "jumpSensor");
                jumpSensor.CollisionDetected += this.SensorCollision;
                jumpSensor.SeparationDetected += this.SensorSeparation;
                this.AddSensor(jumpSensor);
            }
        }

        private bool CanJump
        {
            get
            {
                return this.onGround && (Math.Abs(this.Body.LinearVelocity.Y) < 0.5);
            }
        }

        public void Update(GameTime time, KeyboardState ks)
        {
            string spriteStateString = string.Empty;
            Vector2 dp = Vector2.Zero;

            if (ks.IsKeyDown(KeyboardController.UpKey))
            {
                spriteStateString = "Up";
            }
            else if (ks.IsKeyDown(KeyboardController.DownKey))
            {
                spriteStateString = "Down";
            }
            else if (ks.IsKeyDown(KeyboardController.LeftKey))
            {
                dp.X -= this.MovementSpeed.X;
                spriteStateString = "Left";
            }
            else if (ks.IsKeyDown(KeyboardController.RightKey))
            {
                dp.X += this.MovementSpeed.X;
                spriteStateString = "Right";
            }
            else if (this.CanJump && ks.IsKeyDown(KeyboardController.JumpKey))
            {
                spriteStateString = "Jump";
                dp.Y -= 30;
            }
            else
            {
                spriteStateString = "Idle";
            }
            
            dp.Y = Math.Abs(this.Body.LinearVelocity.Y + dp.Y) >= this.MaxSpeed.Y ? 0 : dp.Y;
            dp.X = Math.Abs(this.Body.LinearVelocity.X + dp.X) >= this.MaxSpeed.X ? 0 : dp.X;

            this.Body.LinearVelocity += dp;

            foreach (var sprite in this.Sprites.Values)
            {
                sprite.CurrentAnimationString = spriteStateString;
            }
            
            base.Update(time);
        }

        public new XElement XmlSerialize()
        {
            return new XElement("Player", new XAttribute("class", typeof(Player).ToString()), base.XmlSerialize());
        }

        public new void XmlDeserialize(XElement element)
        {            
            if (element != null)
            {
                base.XmlDeserialize(element.Element("Actor"));
            }
        }

        private bool Body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {            
            return true;
        }

        private bool SensorCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            this.onGround = true;
            return true;
        }

        private void SensorSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            this.onGround = false;
        }
    }
}
