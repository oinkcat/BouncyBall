using System;
using System.IO;
using System.Timers;
using System.Reflection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using MauiLib;

namespace BouncyBall;

public partial class App : Application
{
	public App()
	{
		var resNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
		Array.ForEach(resNames, Console.WriteLine);

		Ui.Orientation = ScreenOrientation.Portrait;

		InitializeComponent();
		
        var mainNavPage = new NavigationPage(new FrontPage());
        mainNavPage.BarTextColor = Colors.White;
        mainNavPage.BarBackgroundColor = Colors.DarkOrange;
        MainPage = mainNavPage;
	}
}
