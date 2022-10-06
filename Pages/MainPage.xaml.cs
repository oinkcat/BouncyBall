﻿using System;
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
		
		private (double, double)? touchCoords;
		
		private (double, double)? moveCoords;
		
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
			
			var touchArea = new TouchView();
			touchArea.TouchStart += TouchStarted;
			touchArea.TouchMove += TouchMoved;
			touchArea.TouchEnd += TouchCompleted;
			
			layout.Children.Add(touchArea);
		}
		
		private void TouchStarted(object sender, (double, double) coords)
		{
			touchCoords = coords;
			moveCoords = touchCoords;
			
			var (tx, ty) = touchCoords.Value;
			Console.WriteLine($"{tx}, {ty}");
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
				layout.Children.Insert(0, blockFrame);
				PlaceBlock(newBlock, blockFrame);
				entityBlocks.Add(newBlock, blockFrame);
			}
			
			layout.RaiseChild(ball);
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
			foreach(var (block, frame) in entityBlocks)
			{
				PlaceBlock(block, frame);
				frame.BackgroundColor = (block == game.Collision)
					? Color.Green
					: Color.Blue;
			}
			
			PlaceBlock(game.Ball, ball);
		}
	}
}