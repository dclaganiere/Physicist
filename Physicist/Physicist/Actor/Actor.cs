namespace Physicist.Actors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Factories;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Physicist.Extensions;

    public class Actor
    {
        private Dictionary<string, GameSprite> sprites = new Dictionary<string, GameSprite>();
        private Dictionary<string, CollisionSensor> sensors = new Dictionary<string,CollisionSensor>();
        private Body body;

        public Actor()
        {            
            this.VisibleState = Visibility.Visible;
            this.IsEnabled = true;
            this.Health = 1;
        }

        public IEnumerable<CollisionSensor> Sensors
        {
            get
            {
                return this.sensors.Values;
            }
        }

        // Farseer Structures
        public Body Body 
        { 
            get
            {
                return this.body;
            }

            set
            {
                this.body = value;
            }
        }

        // 2space variables
        public Vector2 Position 
        {
            get
            {
                return this.body.Position;
            }

            set
            {
                this.body.Position = value;
            }
        }
        
        public float Rotation 
        {
            get
            {
                return this.body.Rotation;
            }

            set
            {
                this.body.Rotation = value;
            }
        }

        public Vector2 MovementSpeed { get; set; }

        public Vector2 MaxSpeed { get; set; }

        // gameplay state variables
        public int Health { get; set; }
        
        public bool IsEnabled { get; set; }
        
        public bool IsDead 
        {
            get
            {
                return this.Health <= 0;
            }
        }

        // draw properties
        public Dictionary<string, GameSprite> Sprites 
        {
            get
            {
                return this.sprites;
            }
        }

        public Visibility VisibleState { get; set; }

        public virtual void Draw(SpriteBatch sb)
        {
            if (this.IsEnabled)
            {
                if (sb != null && this.VisibleState == Visibility.Visible)
                {
                    foreach (var sprite in this.Sprites.Values)
                    {
                        Vector2 shapeOffset = (Vector2)sprite.FrameSize / 2.0f;

                        var effect = SpriteEffects.None;
                        if (sprite.CurrentAnimation.FlipHorizontal)
                        {
                            effect |= SpriteEffects.FlipHorizontally;
                        }

                        if (sprite.CurrentAnimation.FlipVertical)
                        {
                            effect |= SpriteEffects.FlipVertically;
                        }

                        sb.Draw(
                            sprite.SpriteSheet,
                            this.Position.ToDisplayUnits() + sprite.Offset - shapeOffset,
                            sprite.CurrentSprite,
                            Color.White,
                            this.Rotation,
                            Vector2.Zero,
                            1f,
                            effect,
                            sprite.Depth);

                    }
                }
            }
        }

        public virtual void AddSprite(string name, GameSprite sprite)
        {
            if (sprite == null)
            {
                throw new ArgumentNullException("sprite");
            }

            this.Sprites.Add(name, sprite);
        }

        public virtual void Update(GameTime time)
        {           
            // update every sprite in the sprite collection
            if (this.IsEnabled)
            {
                foreach (var sprite in this.Sprites.Values)
                {
                    sprite.Update(time);
                }
            }
        }

        protected void AddSensor(CollisionSensor sensor)
        {
            this.sensors.Add(sensor.SensorName, sensor);
        }
    }
}
