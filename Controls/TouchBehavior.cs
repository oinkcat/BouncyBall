using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Android.Views;

using AView = Android.Views.View;

namespace BouncyBall;

public class TouchBehavior : PlatformBehavior<TouchView, AView>
{
    private TouchView view;
    
    protected override void OnAttachedTo(TouchView bindable, AView platformView)
    {
        base.OnAttachedTo(bindable, platformView);
        
        view = bindable;
        platformView.Touch += View_Touch;
    }
    
    private void View_Touch(object sender, AView.TouchEventArgs e)
    {
        (double, double) getDipEventCoords()
        {
            var dpi = DeviceDisplay.MainDisplayInfo.Density;
            return (e.Event.GetX(0) / dpi, (e.Event.GetY(0) / dpi));
        }
        
        var (tX, tY) = getDipEventCoords();

        if (e.Event.Action == MotionEventActions.Down)
        {
            view.OnTouchEvent(new TouchInfo(TouchEvent.Tap, tX, tY));
            e.Handled = true;
        }
        else if (e.Event.Action == MotionEventActions.Move)
        {
            view.OnTouchEvent(new TouchInfo(TouchEvent.Move, tX, tY));
            e.Handled = true;
        }
        else if (e.Event.Action == MotionEventActions.Up)
        {
            view.OnTouchEvent(new TouchInfo(TouchEvent.Release, tX, tY));
            e.Handled = true;
        }
    }
    
    protected override void OnDetachedFrom(TouchView bindable, AView platformView)
    {
        view = null;
        platformView.Touch -= View_Touch;
        
        base.OnDetachedFrom(bindable, platformView);
    }
}
