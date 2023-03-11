using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall
{
	/// <summary>
	/// Block obstacle
	/// </summary>
    public class Block : Entity
    {
    	public PlayerBall StandingBall { get; set; }
    	
    	public Block(double x, double y, double width, double height)
    		: base(x, y, width, height) { }
    	
        public override void Update()
        {
        	StandingBall = null;
        }
    }
}