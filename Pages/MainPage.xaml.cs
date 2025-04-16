using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using BouncyBall.Game;

namespace BouncyBall;

/// <summary>
/// Presentation logic
/// </summary>
public partial class MainPage : ContentPage
{
	private Timer moveTimer;

	private readonly GameLogic game;

	private (double, double)? touchCoords;

	private (double, double)? moveCoords;
    
    private int randomLevel;

	public MainPage(GameLogic game)
	{
		NavigationPage.SetHasNavigationBar(this, false);

		this.game = game;
        randomLevel = Settings.Instance.RandomnessLevel;
		game.GameOver += HandleGameOver;

		InitializeComponent();
        
        var drawable = new GameDrawable(game);

		SizeChanged += (s, e) =>
		{
			if (!game.IsStarted)
			{
				game.Initialize(Width, Height);
                drawable.Initialize(Width, Height, Settings.Instance.SkinName);
				Dispatcher.Dispatch(ReStartGame);
			}
		};
        
        Unloaded += (s, e) => moveTimer.Stop();
        
        GameGraphics.Drawable = drawable;

		touchArea.TouchStart += TouchStarted;
		touchArea.TouchMove += TouchMoved;
		touchArea.TouchEnd += TouchCompleted;
	}

	private void ReStartGame()
	{
		StartBall();
	}


	private void TouchStarted(object sender, (double, double) coords)
	{
		touchCoords = coords;
		moveCoords = touchCoords;
	}

	private void TouchMoved(object sender, (double, double) coords)
	{
		moveCoords = coords;
	}

	private void TouchCompleted(object sender, EventArgs e)
	{
		if (touchCoords.HasValue)
		{
			var (tx, ty) = touchCoords.Value;
			var (mx, my) = moveCoords.Value;
			game.Interact(tx - mx, ty - my);
			touchCoords = null;
		}
	}

	private void StartBall()
	{
		moveTimer = new Timer(30);
		moveTimer.Elapsed += UpdateState;
		moveTimer.Start();

		UpdateState(this, EventArgs.Empty);
	}

	private void UpdateState(object sender, EventArgs e)
	{
		game.Update();
		GameGraphics.Invalidate();
    }

	private void HandleGameOver(object sender, EventArgs e)
	{
		Dispatcher.Dispatch(async () =>
		{
			moveTimer.Stop();

			int rank = GetPlayerRank();

			var msgBuilder = new StringBuilder("Your final score: ");
			msgBuilder.AppendLine(game.Score.ToString());

			if (rank > 0)
			{
				msgBuilder.Append($"#{rank} high score!");
			}

			bool doRetry = await DisplayAlert("Game Over", msgBuilder.ToString(), "Retry", "End");

			if (doRetry)
			{
				game.Initialize(Width, Height);
				ReStartGame();
			}
			else
			{
				await Navigation.PopModalAsync();
			}
		});
	}

	private int GetPlayerRank()
	{
		var scoreTable = new HighScoreTable();
		scoreTable.Load();

		if (scoreTable.TryStorePlayerScore(game.Score, out int rank))
		{
			scoreTable.Save();
		}

		return rank;
	}

	protected override void OnAppearing()
	{
		if (moveTimer != null)
		{
			moveTimer.Enabled = true;
			Console.WriteLine("Resume");
		}
	}

	protected override void OnDisappearing()
	{
		moveTimer.Enabled = false;
		Console.WriteLine("Pause");
	}
}
