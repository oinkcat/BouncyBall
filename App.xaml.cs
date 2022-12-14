using System;
using System.IO;
using System.Timers;
using System.Reflection;
using Xamarin.Forms;

namespace BouncyBall
{
	public partial class App : Application
	{
		public App()
		{
			var resNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
			Array.ForEach(resNames, Console.WriteLine);
			
			PlatformUtils.SetPortraitMode();
			
			InitializeComponent();
			MainPage = new NavigationPage(new FrontPage());
		}
	}
}