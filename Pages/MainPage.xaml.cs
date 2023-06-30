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

	private readonly RecyclingEntitiesPool entitiesPool;

	private (double, double)? touchCoords;

	private (double, double)? moveCoords;

	public MainPage(GameLogic game)
	{
		NavigationPage.SetHasNavigationBar(this, false);

		this.game = game;

		game.ObjectCreated += HandleNewObject;
		game.ObjectRemoved += HandleRemovedObject;
		game.GameOver += HandleGameOver;

		entitiesPool = new RecyclingEntitiesPool();

		InitializeComponent();

		this.SizeChanged += (s, e) =>
		{
			if (!game.IsStarted)
			{
				game.Initialize(Width, Height);
				Dispatcher.Dispatch(ReStartGame);
			}
		};

		touchArea.TouchStart += TouchStarted;
		touchArea.TouchMove += TouchMoved;
		touchArea.TouchEnd += TouchCompleted;

		ballImage.Source = ImageSource.FromResource("BouncyBall.resources.oink.png");
		backImage.SetImageByName("BouncyBall.resources.back.png");
	}

	private void ReStartGame()
	{
		entitiesPool.Reset();

		var blocksToRemove = layout.Children
			.OfType<Frame>()

			.Where(b => b != ball)
			.ToArray();

		foreach (var view in blocksToRemove)
		{
			layout.Remove(view);
		}

		SpawnBlocks(game.Obstacles);
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

	private void HandleNewObject(object sender, Entity newObject)
	{
		entitiesPool.EntityAdded(newObject);
	}

	private void HandleRemovedObject(object sender, Entity removedObject)
	{
		Dispatcher.Dispatch(() => entitiesPool.GetFrame(removedObject).IsVisible = false);
		entitiesPool.EntityRemoved(removedObject);
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
		Dispatcher.Dispatch(() =>
		{
			MoveObjects();
			entitiesPool.HandleBlocks(SpawnBlocks, frame => layout.Remove(frame));
			DisplayInfo();
		});
	}

	private void SpawnBlocks(IEnumerable<Entity> blocks)
	{
		foreach (var newBlock in blocks)
		{
			var blockFrame = entitiesPool.CreateOrRecycleFrame(newBlock, out bool old);

			if (old)
			{
				blockFrame.IsVisible = true;
			}
			else
			{
				layout.Insert(layout.Count, blockFrame);
			}

			PlaceBlock(newBlock, blockFrame);
		}
	}

	private void PlaceBlock(Entity block, Frame frame)
	{
		double blockY = Height - block.Y - block.Height;
		AbsoluteLayout.SetLayoutBounds(frame, new Rect
		{
			X = block.X,
			Y = blockY + game.BaseLine,
			Width = block.Width,
			Height = block.Height
		});
		frame.ZIndex = -10;
	}

	private void MoveObjects()
	{
		if (game.BaseLine > 10)
		{
			backImage.Offset = game.BaseLine / 2;
		}

		foreach (var (block, frame) in entitiesPool.GetAllEntities())
		{
			PlaceBlock(block, frame);
		}

		PlaceBlock(game.Ball, ball);
	}

	private void DisplayInfo()
	{
		ScoreLabel.Text = game.Score.ToString().PadLeft(5, '0');
		leftLabel.Text = game.Ball.JumpsLeft.ToString();
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
