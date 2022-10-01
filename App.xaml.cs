using System;
using System.IO;
using System.Timers;
using Xamarin.Forms;

namespace BouncyBall
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			MainPage = new MainPage(new Game());
		}
	}
}