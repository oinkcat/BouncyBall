using System;
using Android.Views;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

using XPlatform = Microsoft.Maui.ApplicationModel.Platform;
using AView = Android.Views.View;

namespace BouncyBall
{
	/// <summary>
	/// Element that supports touch events
	/// </summary>
    public class TouchView : ContentView
    {
        public event EventHandler<(double, double)> TouchStart;

        public event EventHandler<(double, double)> TouchMove;

        public event EventHandler TouchEnd;

        private AView nativeView;
        
        private double dpi;

        public TouchView()
        {
            nativeView = new AView(XPlatform.CurrentActivity);
			nativeView.SetMinimumWidth(-1);
			nativeView.SetMinimumHeight(-1);
			nativeView.Touch += View_Touch;
            
            dpi = DeviceDisplay.MainDisplayInfo.Density;

            Content = nativeView.ToView();
        }

        private void View_Touch(object sender, AView.TouchEventArgs e)
        {
        	(double, double) getDipEventCoords()
        	{
        		return (e.Event.GetX(0) / dpi, (e.Event.GetY(0) / dpi));
        	}
        	
            if(e.Event.Action == MotionEventActions.Down)
            {
                TouchStart?.Invoke(this, getDipEventCoords());
                e.Handled = true;
            }
            else if(e.Event.Action == MotionEventActions.Move)
            {
                TouchMove?.Invoke(this, getDipEventCoords());
                e.Handled = true;
            }
            else if(e.Event.Action == MotionEventActions.Up)
            {
                TouchEnd?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
            }
        }
    }
}