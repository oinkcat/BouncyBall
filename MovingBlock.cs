using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace BouncyBall
{
	/// <summary>
	/// Block that can move horizontally
	/// <//summary>
    public class MovingBlock : Block
    {
    	public MovingBlock(double x, double y, double size)
    		: base(x, y, size, size)
    	{
    		Accelerate(2.0, 0.0);
    	}
    		
        public override void Update()
        {
        	if(Bump?.IsSide ?? false)
        	{
        		MoveBy(-Velocity.X, 0.0);
    			Accelerate(-Velocity.X * 2, 0.0);
        	}
        }
    }
}