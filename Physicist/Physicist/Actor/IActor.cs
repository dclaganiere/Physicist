﻿namespace Physicist.Actors
{
    using System.Collections.Generic;
    using FarseerPhysics.Dynamics;
    using Microsoft.Xna.Framework;
    using Physicist.Controls;
    
    public interface IActor : IPosition, IName, IUpdate, IDraw
    {
        bool IsEnabled { get; set; }

        Body Body { get; set; }

        int Health { get; set; }
        
        Visibility VisibleState { get; set; }
        
        bool IsDead { get; }
        
        Dictionary<string, GameSprite> Sprites { get; }
        
        Vector2 MovementSpeed { get; set; }
    }
}
