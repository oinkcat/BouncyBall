using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Essentials;
using Android;
using Android.Content.PM;

namespace BouncyBall
{
    public static class PlatformUtils 
    {
    	private static Android.App.Activity act;
    	
    	public static void ToggleAlternateTheme()
    	{
			act.SetTheme(Android.Resource.Style.ThemeMaterial);
    	}
    	
    	public static void SetPortraitMode()
    	{
			act.RequestedOrientation = ScreenOrientation.Portrait;
    	}
    	
    	static PlatformUtils()
    	{
    		act = Platform.CurrentActivity;
    	}
    }
}