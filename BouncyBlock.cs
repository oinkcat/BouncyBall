using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall
{
	/// <summary>
	/// Block that can be bouncy to player
	/// </summary>
    public class BouncyBlock : Block
    {
        public BouncyBlock(double x, double y, double size)
    		: base(x, y, size, size) { }
    		
    	public override void Update()
    	{
    		if(StandingBall != null)
    		{
    			StandingBall.Stand = null;
    			StandingBall.Accelerate(0.0f, 15.0f);
    		}
    		
    		base.Update();
    	}
    }
}