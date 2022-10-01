using System;
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
		
		private Dictionary<Entity, Frame> entityBlocks;
		
		public MainPage(Game game)
		{
			this.game = game;
			
			game.ObjectCreated += HandleNewObject;
			game.ObjectRemoved += HandleRemovedObject;
			
			entityBlocks = new Dictionary<Entity, Frame>();
			
			InitializeComponent();
			StartBall();
			
			this.SizeChanged += (s, e) => {
				if(!game.IsStarted)
				{
					game.Initialize(Width, Height);
					Dispatcher.BeginInvokeOnMainThread(() => SpawnBlocks(game.Obstacles));
				}
			};
		}
		
		private void HandleNewObject(object sender, Entity newObject)
		{
			Dispatcher.BeginInvokeOnMainThread(() => {
				SpawnBlocks(new [] { newObject });
			});
		}
		
		private void HandleRemovedObject(object sender, Entity removedObject)
		{
			var blockFrame = entityBlocks[removedObject];
			Dispatcher.BeginInvokeOnMainThread(() => layout.Children.Remove(blockFrame));
			entityBlocks.Remove(removedObject);
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
			Dispatcher.BeginInvokeOnMainThread(MoveObjects);
		}
		
		private void SpawnBlocks(IList<Entity> blocks)
		{
			foreach(var newBlock in blocks)
			{
				var blockFrame = new Frame();
				blockFrame.BackgroundColor = Color.Blue;
				blockFrame.BorderColor = Color.Black;
				layout.Children.Add(blockFrame);
				PlaceBlock(newBlock, blockFrame);
				entityBlocks.Add(newBlock, blockFrame);
			}
			
			layout.RaiseChild(ball);
		}
		
		private void PlaceBlock(Entity block, Frame frame)
		{
			double blockY = Height - (block.Y - game.BaseLine);
			AbsoluteLayout.SetLayoutBounds(frame, new Rectangle {
				X = block.X,
				Y = blockY,
				Width = block.Width,
				Height = block.Height
			});
		}
		
		private void MoveObjects()
		{
			foreach(var (block, frame) in entityBlocks)
			{
				PlaceBlock(block, frame);
			}
			
			AbsoluteLayout.SetLayoutBounds(ball, new Rectangle {
				X = game.Ball.X,
				Y = Height - game.Ball.Y,
				Width = game.Ball.Width,
				Height = game.Ball.Height
			});
		}
	}
}