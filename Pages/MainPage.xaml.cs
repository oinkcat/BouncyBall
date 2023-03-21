using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace BouncyBall
{
	/// <summary>
	/// Presentation logic
	/// </summary>
	public partial class MainPage : ContentPage
	{
		private Timer moveTimer;

		private readonly Game game;

		private readonly Dictionary<Entity, Frame> entityElems;

		private readonly List<Entity> createdBlocks;

		private readonly List<Entity> removedBlocks;

		private readonly ImageSource[] brickImages;

		private (double, double)? touchCoords;

		private (double, double)? moveCoords;

		public MainPage(Game game)
		{
			NavigationPage.SetHasNavigationBar(this, false);

			this.game = game;

			game.ObjectCreated += HandleNewObject;
			game.ObjectRemoved += HandleRemovedObject;
			game.GameOver += HandleGameOver;

			brickImages = Enumerable.Range(1, 3)
				.Select(i =>
				{
					string resName = $"BouncyBall.resources.brick{i}.png";
					return ImageSource.FromResource(resName);
				})
				.ToArray();

			entityElems = new Dictionary<Entity, Frame>();
			createdBlocks = new List<Entity>();
			removedBlocks = new List<Entity>();

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
			entityElems.Clear();
			createdBlocks.Clear();
			removedBlocks.Clear();

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
			createdBlocks.Add(newObject);
		}

		private void HandleRemovedObject(object sender, Entity removedObject)
		{
			removedBlocks.Add(removedObject);
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
				HandleCreatedAndRemovedBlocks();
				DisplayInfo();
			});
		}

		private void SpawnBlocks(IList<Entity> blocks)
		{
			foreach (var newBlock in blocks)
			{
				var blockFrame = CreateOrRecycleFrame(newBlock, out bool old);

				if (!old)
				{	
					layout.Insert(layout.Count, blockFrame);
				}

				PlaceBlock(newBlock, blockFrame);
				entityElems.Add(newBlock, blockFrame);
			}
		}

		private Frame CreateOrRecycleFrame(Entity block, out bool recycled)
		{
			var recycledBlock = removedBlocks
				.FirstOrDefault(b => b.GetType() == block.GetType());
			recycled = recycledBlock != null;

			if (recycled)
			{
				removedBlocks.Remove(recycledBlock);
				var recycledFrame = entityElems[recycledBlock];
				entityElems.Remove(recycledBlock);

				return recycledFrame;
			}
			else
			{
				return new Frame()
				{
					BackgroundColor = Colors.Gray,
					BorderColor = Colors.Black,
					Padding = new Thickness(1),
					Content = new Image()
					{
						Aspect = Aspect.Fill,
						Source = block switch
						{
							MovingBlock => brickImages[1],
							BouncyBlock => brickImages[2],
							_ => brickImages[0]
						}
					}
				};
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
			if(game.BaseLine > 10)
			{
				backImage.Offset = game.BaseLine / 2;
			}
			
			foreach (var (block, frame) in entityElems)
			{
				PlaceBlock(block, frame);
			}

			PlaceBlock(game.Ball, ball);
		}

		private void HandleCreatedAndRemovedBlocks()
		{
			// Place new blocks
			if (createdBlocks.Count > 0)
			{
				SpawnBlocks(createdBlocks);
				createdBlocks.Clear();
			}

			// Remove blocks
			if (removedBlocks.Count < 20) { return; }

			foreach (var removedBlock in removedBlocks)
			{
				var blockFrame = entityElems[removedBlock];
				layout.Remove(blockFrame);
				entityElems.Remove(removedBlock);
			}

			removedBlocks.Clear();
			Console.WriteLine($"Clear at {Environment.TickCount}");
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
	}
}