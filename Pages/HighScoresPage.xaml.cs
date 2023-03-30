using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace BouncyBall
{
	public partial class HighScoresPage : ContentPage
	{
		public List<Tuple<int, HighScoreEntry>> Scores { get; }
		
		public HighScoresPage()
		{
			NavigationPage.SetHasBackButton(this, true);
			
			InitializeComponent();
			Scores = GetHighScores();
			
			BindingContext = this;
		}
		
		private List<Tuple<int, HighScoreEntry>> GetHighScores()
		{
			var scoreTable = new HighScoreTable();
			scoreTable.Load();
			
			return scoreTable.Scores
				.Select((e, i) => Tuple.Create(i + 1, e))
				.ToList();
		}
		
		protected override bool OnBackButtonPressed()
		{
			base.OnBackButtonPressed();
			
			Navigation.PopAsync();
			return true;
		}
	}
}