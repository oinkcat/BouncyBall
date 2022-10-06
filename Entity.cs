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
    	private const float MaxVelocity = 10;
    	
        public Vector2 Position { get; private set; }
        
        public Vector2 Velocity { get; private set; }
        
        public int Width { get; }
        
        public int Height { get; }
        
        public double X => (double)Position.X;
        
        public double Y => (double)Position.Y;
        
        public bool Jumping => Velocity.Y > 0.0f;
        
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
        
        public void MoveBy(double dx, double dy)
        {
        	Position += new Vector2((float)dx, (float)dy);
        }
        
        public void MoveOnEntity(Entity another)
        {
        	float newY = (float)another.Y + another.Height;
        	Position = new Vector2((float)X, newY);
        }
        
        public void Accelerate(double ax, double ay)
        {
        	var newVelocity = Velocity + new Vector2((float)ax, (float)ay);
        	
        	float vx = Math.Clamp(newVelocity.X, -MaxVelocity, MaxVelocity);
        	float vy = Math.Clamp(newVelocity.Y, -MaxVelocity, MaxVelocity);
        	Velocity = new Vector2(vx, vy);
        }
        
        public void Move()
        {
        	Position += Velocity;
        }
        
        public void Stop()
        {
        	Velocity = new Vector2(0.0f);
        }
    }
}