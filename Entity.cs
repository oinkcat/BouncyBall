using System;
using System.Numerics;
using System.Collections.Generic;

namespace BouncyBall
{
	/// <summary>
	/// Game entity
	/// </summary>
    public class Entity 
    {         
        public Vector2 Position { get; private set; }
        
        public Vector2 Velocity { get; private set; }
        
        public int Width { get; }
        
        public int Height { get; }
        
        public double X => (double)Position.X;
        
        public double Y => (double)Position.Y;
        
        public Entity(double x, double y, double width, double height)
        {
        	Position = new Vector2((float)x, (float)y);
        	Velocity = new Vector2(0);
        	
        	Width = (int)width;
        	Height = (int)height;
        }
        
        public void MoveTo(double x, double y)
        {
        	Position = new Vector2((float)x, (float)y);
        }
    }
}