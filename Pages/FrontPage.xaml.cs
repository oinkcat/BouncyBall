using System;
using System.IO;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace BouncyBall
{
	public partial class FrontPage : ContentPage
	{
		public FrontPage()
		{
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
		}
		
		private async void Play_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new MainPage(new Game()));
		}
		
		private async void HS_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new HighScoresPage());
		}
		
		private async void Exit_Clicked(object sender, EventArgs e)
		{
			Environment.Exit(0);
		}
	}
}