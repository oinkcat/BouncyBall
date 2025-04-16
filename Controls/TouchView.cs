using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Android.Widget;

namespace BouncyBall;

/// <summary>
/// Element with touch events
/// </summary>
public class TouchView : ContentView
{         
    public event EventHandler<(double, double)> TouchStart;

    public event EventHandler<(double, double)> TouchMove;

    public event EventHandler TouchEnd;
    
    internal void OnTouchEvent(TouchInfo info)
    {
        switch(info.Type)
        {
            case TouchEvent.Tap:
                TouchStart?.Invoke(this, (info.X, info.Y));
                break;
            case TouchEvent.Move:
                TouchMove?.Invoke(this, (info.X, info.Y));
                break;
            case TouchEvent.Release:
                TouchEnd?.Invoke(this, EventArgs.Empty);
                break;
        }
    }
}
