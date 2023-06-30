using System;
using System.IO;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using BouncyBall.Game;

namespace BouncyBall;

public partial class FrontPage : ContentPage
{
	public FrontPage()
	{
		InitializeComponent();
		NavigationPage.SetHasNavigationBar(this, false);
	}

	private async void Play_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new MainPage(new GameLogic()));
	}

	private async void HS_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new HighScoresPage());
	}

	private void Exit_Clicked(object sender, EventArgs e)
	{
		Application.Current.Quit();
	}
}
