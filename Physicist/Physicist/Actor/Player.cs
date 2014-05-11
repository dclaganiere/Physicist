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
        private static readonly float MAX_SPEED = 4f;
        public Player(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            this.MaxSpeed = new Vector2(MAX_SPEED, MAX_SPEED);
            this.XmlDeserialize(element);
        }

        public Player() :
            base()
        {
            this.MaxSpeed = new Vector2(MAX_SPEED, MAX_SPEED);
        }

        public Vector2 MaxSpeed { get; private set; }

        public new Body Body
        {
            get
            {
                return base.Body;
            }

            set
            {
                if (base.Body != value)
                {
                    base.Body.OnCollision -= this.Body_OnCollision;
                    base.Body = value;

                    this.Body.LinearDamping = 1.2f;

                    this.SetUpCollisions();
                }
            }
        }

        private bool CanJump
        {
            get
            {
                return this.onGround && (Math.Abs(this.Body.LinearVelocity.Y) < 0.005);
            }
        }

        private bool isJumping;
        private int jumpTime = 0;
        public void Update(GameTime time, KeyboardState ks)
        {
            string spriteStateString = string.Empty;
            Vector2 dp = Vector2.Zero;
            


            spriteStateString = "Idle";


            if (ks.IsKeyDown(KeyboardController.UpKey))
            {
                spriteStateString = "Up";
                this.Body.ApplyLinearImpulse(new Vector2(0, -500 * this.MovementSpeed.Y));
            }

            if (ks.IsKeyDown(KeyboardController.DownKey))
            {
                spriteStateString = "Down";
            }

            if (ks.IsKeyDown(KeyboardController.LeftKey))
            {
                this.Body.ApplyForce(new Vector2(-15 * this.MovementSpeed.X, 0));
                spriteStateString = "Left";
            }
            else if (ks.IsKeyDown(KeyboardController.RightKey))
            {
                this.Body.ApplyForce(new Vector2(15 * this.MovementSpeed.X, 0));
                spriteStateString = "Right";
            }

            if (ks.IsKeyDown(KeyboardController.JumpKey))
            {
                if ((!this.isJumping && this.CanJump) || (this.isJumping && (jumpTime > 0)))
                {
                    if (jumpTime <= 0)
                    {
                        jumpTime = 20;
                        this.Body.ApplyLinearImpulse(new Vector2(0, -4 * this.MovementSpeed.Y));
                    }
                    isJumping = true;
                    this.Body.ApplyForce(new Vector2(0, -10 * this.MovementSpeed.Y));
                    jumpTime--;
                    spriteStateString = "Up";
                }
            }
            else
            {
                isJumping = false;
                jumpTime = 0;
            }

            Console.WriteLine("({0:0.00}, {1:0.00})", this.Body.LinearVelocity.X, this.Body.Position.Y);


            //if (ks.IsKeyDown(KeyboardController.UpKey))
            //{
            //    spriteStateString = "Up";
            //}

            //if (ks.IsKeyDown(KeyboardController.DownKey))
            //{
            //    spriteStateString = "Down";
            //}

            //if (ks.IsKeyDown(KeyboardController.LeftKey))
            //{
            //    dp.X -= this.MovementSpeed.X;
            //    spriteStateString = "Left";
            //}

            //if (ks.IsKeyDown(KeyboardController.RightKey))
            //{
            //    dp.X += this.MovementSpeed.X;
            //    spriteStateString = "Right";
            //}

            //if (ks.IsKeyDown(KeyboardController.JumpKey))
            //{
            //    if((!this.isJumping && this.CanJump) || (this.isJumping && (jumpTime > 0)))
            //    {
            //        if(jumpTime <= 0)
            //        {
            //            jumpTime = 10;
            //        }
            //        isJumping = true;
            //        dp.Y -= 2;
            //        Console.WriteLine("We jumping!");
            //        jumpTime--;
            //    }
            //}
            //else
            //{
            //    isJumping = false;
            //}


            //if ((this.Body.LinearVelocity.Y + dp.Y) < 0)
            //{
            //    if ((this.Body.LinearVelocity.Y + dp.Y) >= -this.MaxSpeed.Y)
            //    {
            //        dp.Y = this.Body.LinearVelocity.Y + dp.Y;
            //    }
            //    else
            //    {
            //        dp.Y = -this.MaxSpeed.Y;
            //    }
            //}
            //else
            //{
            //    dp.Y = this.Body.LinearVelocity.Y;
            //}

            //if ((this.Body.LinearVelocity.X + dp.X) < 0)
            //{
            //    if ((this.Body.LinearVelocity.X + dp.X) >= -this.MaxSpeed.X)
            //    {
            //        dp.X = this.Body.LinearVelocity.X + dp.X;
            //    }
            //    else
            //    {
            //        dp.X = -this.MaxSpeed.X;
            //    }
            //}
            //else
            //{
            //    if ((this.Body.LinearVelocity.X + dp.X) <= this.MaxSpeed.X)
            //    {
            //        dp.X = this.Body.LinearVelocity.X + dp.X;
            //    }
            //    else
            //    {
            //        dp.X = this.MaxSpeed.X;
            //    }
            //}

            //Console.WriteLine("x={0}, y={1}", dp.X, dp.Y);

            //this.Body.LinearVelocity = dp;

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

            this.SetUpCollisions();
        }

        private void SetUpCollisions()
        {
            this.ClearSensors();

            this.Body.OnCollision += this.Body_OnCollision;

            // *************************DIMENSIONS ARE TEST VALUES*********************************** //
            this.AddCollisionSensor(
                "jumpSensor",
                FixtureFactory.AttachRectangle(10, 3, 1f, new Vector2(0, 19), this.Body));
        }

        private void AddCollisionSensor(string fixtureName, Fixture fixture)
        {
            CollisionSensor jumpSensor = new CollisionSensor(fixture, fixtureName);
            jumpSensor.CollisionDetected += this.SensorCollision;
            jumpSensor.SeparationDetected += this.SensorSeparation;

            this.AddSensor(jumpSensor);
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
