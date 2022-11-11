using System;
using System.Linq;
using System.Collections.Generic;
using System.Timers;
using Xamarin.Forms;

namespace BouncyBall
{
	/// <summary>
	/// Presentation logic
	/// </summary>
	public partial class MainPage : ContentPage
	{
		private Timer moveTimer;
		
		private Game game;
		
		private Dictionary<Entity, Frame> entityElems;
		
		private List<Entity> createdBlocks;
		
		private List<Entity> removedBlocks;
		
		private ImageSource[] brickImages;
		
		private (double, double)? touchCoords;
		
		private (double, double)? moveCoords;
		
		public MainPage(Game game)
		{
			this.game = game;
			
			game.ObjectCreated += HandleNewObject;
			game.ObjectRemoved += HandleRemovedObject;
			game.GameOver += HandleGameOver;
			
			brickImages = Enumerable.Range(0, 2)
				.Select(i => {
					string resName = $"BouncyBall.resources.brick{i + 1}.png";
					return ImageSource.FromResource(resName);
				})
				.ToArray();
			
			entityElems = new Dictionary<Entity, Frame>();
			createdBlocks = new List<Entity>();
			removedBlocks = new List<Entity>();
			
			InitializeComponent();
			
			this.SizeChanged += (s, e) => {
				if(!game.IsStarted)
				{
					game.Initialize(Width, Height);
					Dispatcher.BeginInvokeOnMainThread(ReStartGame);
				}
			};
			
			var touchArea = new TouchView();
			touchArea.TouchStart += TouchStarted;
			touchArea.TouchMove += TouchMoved;
			touchArea.TouchEnd += TouchCompleted;
			layout.Children.Add(touchArea);
			
			ballImage.Source = ImageSource.FromResource("BouncyBall.resources.ball.png");
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
				
			foreach(var view in blocksToRemove)
			{
				layout.Children.Remove(view);
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
			if(touchCoords.HasValue)
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
			Dispatcher.BeginInvokeOnMainThread(() =>  {
				MoveObjects();
				HandleCreatedAndRemovedBlocks();
				DisplayInfo();
			});
		}
		
		private void SpawnBlocks(IList<Entity> blocks)
		{
			foreach(var newBlock in blocks)
			{
				var blockFrame = CreateOrRecycleFrame(newBlock, out bool old);
				
				if(!old)
				{
					layout.Children.Insert(0, blockFrame);
				}
				
				
				PlaceBlock(newBlock, blockFrame);
				entityElems.Add(newBlock, blockFrame);
			}
			
			layout.RaiseChild(ball);
		}
		
		private Frame CreateOrRecycleFrame(Entity block, out bool recycled)
		{
			bool isMoving = block is MovingBlock;
			
			var recycledBlock = removedBlocks
				.FirstOrDefault(b => !((b is MovingBlock) ^ isMoving));
			recycled = recycledBlock != null;
				
			if(recycled)
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
					BackgroundColor = Color.Gray,
					BorderColor = Color.Black,
					Padding = new Thickness(1),
					Content = new Image()
					{
						Aspect = Aspect.Fill,
						Source = (block is MovingBlock) 
							? brickImages[1]
							: brickImages[0]
					}
				};
			}
		}
		
		private void PlaceBlock(Entity block, Frame frame)
		{
			double blockY = Height - block.Y - block.Height;
			AbsoluteLayout.SetLayoutBounds(frame, new Rectangle {
				X = block.X,
				Y = blockY + game.BaseLine,
				Width = block.Width,
				Height = block.Height
			});
		}
		
		private void MoveObjects()
		{
			foreach(var (block, frame) in entityElems)
			{
				PlaceBlock(block, frame);
			}
			
			PlaceBlock(game.Ball, ball);
		}
		
		private void HandleCreatedAndRemovedBlocks()
		{
			// Place new blocks
			if(createdBlocks.Count > 0)
			{
				SpawnBlocks(createdBlocks);
				createdBlocks.Clear();
			}
			
			// Remove blocks
			if(removedBlocks.Count < 20) { return; }
			
			foreach(var removedBlock in removedBlocks)
			{
				var blockFrame = entityElems[removedBlock];
				layout.Children.Remove(blockFrame);
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
			Dispatcher.BeginInvokeOnMainThread(async () =>
			{
				moveTimer.Stop();
				string msg = $"Your final score: {game.Score}";
				await DisplayAlert("Game Over", msg, "OK");
				
				game.Initialize(Width, Height);
				ReStartGame();
			});
		}
	}
}