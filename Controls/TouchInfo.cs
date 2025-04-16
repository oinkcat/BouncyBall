using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall;

public enum TouchEvent
{
    Tap, Move, Release
}

public record TouchInfo(TouchEvent Type, double X, double Y);