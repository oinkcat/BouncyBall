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
        public bool CheckCollides(Entity one, Entity another)
        {
        	return (one.X + one.Width >= another.X) &&
        		   (another.X + another.Width >= one.X) &&
        		   (one.Y + one.Height >= another.Y) &&
        		   (another.Y + another.Height >= one.Y);
        }
        
        public Entity CheckCollidesAny(Entity one, IList<Entity> others)
        {
        	foreach(var ent in others)
        	{
        		if(CheckCollides(one, ent))
        		{
        			return ent;
        		}
        	}
        	
        	return null;
        }
    }
}