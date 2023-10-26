using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall.Game;

/// <summary>
/// Block that can collapse
/// </summary>
public class CollapsingBlock : Block
{
    private const int MaxPhase = 3;
    
    private const int CollapseTime = 100;
    
    private const int TimePerPhase = CollapseTime / MaxPhase;
    
    private int time;
    
    public int Phase => Math.Min(time / TimePerPhase, MaxPhase - 1);
    
    public bool IsCollaped => time > CollapseTime;
    
    public CollapsingBlock(double x, double y, double size)
        : base(x, y, size, size)
    {
        time = new Random().Next(CollapseTime / 2);
    }
    
    public override void Update()
    {
        if(StandingBall != null)
        {
            if(!StandingBall.Jumping)
            {
                time++;
            }
            else
            {
                StandingBall = null;
            }
        }
    }
}