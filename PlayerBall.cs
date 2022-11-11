using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall
{
	/// <summary>
	/// Player controlled ball
	/// </summary>
    public class PlayerBall : Entity
    {
        public const int Size = 30;
        
        public int JumpsLeft { get; private set; }
        
        public bool CanJump => JumpsLeft > 0;
        
        public PlayerBall(double x, double y)
        	: base(x, y, (double)Size, (double)Size) { }
        	
        public override void Update()
        {
        	if(Bump != null)
        	{
        		if(Bump.Block != null)
        		{
        			MoveOnEntity(Bump.Block, Bump.Side);
        		}
        		else
        		{
        			double xBySide = (Bump.Side == CollisionSide.Left)
        				? Bump.RightBound.Value - Width
        				: 0;
        			MoveTo(xBySide, Y);
        		}
        		
        		if(Bump.IsSide)
        		{
        			Accelerate(-Velocity.X * 2, 0.0);
        		}
        		else if(Bump.Side == CollisionSide.Bottom)
        		{
        			Accelerate(0.0, -Velocity.Y * 2);
        		}
        		else if(Bump.Side == CollisionSide.Top)
        		{
        			Stop();
        			ResetJumps();
        		}
        	}
        }
        
        public void ResetJumps() => JumpsLeft = 3;
        
        public void Jumped() => JumpsLeft--;
    }
}