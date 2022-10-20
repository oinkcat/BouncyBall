using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall
{
	/// <summary>
	/// Detects collisions of entities
	/// </summary>
    public class CollisionDetector 
    {
    	private double width;
    	
    	public CollisionDetector(double width) => this.width = width;
    	
        public bool CheckCollides(Entity one, Entity another)
        {
        	return (one.X + one.Width >= another.X) &&
        		   (another.X + another.Width >= one.X) &&
        		   (one.Y + one.Height >= another.Y) &&
        		   (another.Y + another.Height >= one.Y);
        }
        
        public Collision CheckCollision(Entity one, IList<Entity> others)
        {
        	if(one.X < 0)
        	{
        		return new Collision { Side = CollisionSide.Right };
        	}
        	else if(one.X + one.Width > width)
        	{
        		return new Collision { Side = CollisionSide.Left };
        	}
        	
        	foreach(var ent in others)
        	{
        		if(CheckCollides(one, ent))
        		{
        			return GetCollisionDetails(one, ent);
        		}
        	}
        	
        	return null;
        }
        
        private Collision GetCollisionDetails(Entity one, Entity another)
        {
        	var collision = new Collision { Block = another };
        	
        	if((one.Velocity.X > 0.0) && (one.X + one.Width > another.X))
        	{
        		collision.Side = CollisionSide.Left;
        	}
        	else if((one.Velocity.X < 0.0) && (one.X < another.X + another.Width))
        	{
        		collision.Side = CollisionSide.Right;
        	}
        	
        	var side = collision.Side;
        	
        	if(side == CollisionSide.Top || side == CollisionSide.Bottom)
        	{
        		collision.Side = side;
        	}
        	else
        	{
        		var vSide = one.Jumping ? CollisionSide.Bottom : CollisionSide.Top;
				var testEntity = new Entity(one.X, one.Y - one.Velocity.Y, one.Width, one.Height);
    			
    			collision.Side = !CheckCollides(testEntity, another)
    				? vSide
    				: side;
        	}
        	
        	return collision;
        }
    }
}