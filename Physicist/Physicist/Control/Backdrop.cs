﻿namespace Physicist.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Physicist.Actors;
    using Physicist.Extensions;

    public class Backdrop : IXmlSerializable
    {
        public Backdrop()
        {
        }

        public Backdrop(Vector2 location, Size dimensions, float depth, Texture2D texture)
        {
            this.Location = location;
            this.Dimensions = dimensions;
            this.Depth = depth;
            this.Texture = texture;
        }

        public Vector2 Location { get; set; }

        public Size Dimensions { get; set; }

        public float Depth { get; set; }

        public Texture2D Texture { get; set; }

        public XElement Serialize()
        {
            XElement element = new XElement(
                "Backdrop",
                this.Location.Serialize("Location"),
                this.Dimensions.Serialize(),
                new XElement("Depth", this.Depth),
                new XElement("TextureRef", this.Texture.Name));

            return element;
        }

        public void Deserialize(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}