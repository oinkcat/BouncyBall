using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall.Game;

/// <summary>
/// Game logic
/// </summary>
public class GameLogic
{
	private const int BlockSize = 60;

	private const int BallSize = 30;

	private const int BlocksInterval = 150;

	private readonly static List<Entity> none = new List<Entity>();

	private (double, double) bounds;

	private int rowCount;

	private readonly Random rng;

	private int tick;

	private int rowNumber;

	private double topBlockYPos;

	private readonly HashSet<MovingBlock> movableBlocks;

	private CollisionDetector detector;

	public bool IsStarted { get; private set; }

	public double BaseLine { get; private set; }

	public int Score { get; private set; }

	public PlayerBall Ball { get; }

	public Entity Stand => Ball.Stand;

	public List<Entity> Obstacles { get; }

	public event EventHandler<Entity> ObjectCreated;

	public event EventHandler<Entity> ObjectRemoved;

	public event EventHandler GameOver;

	public GameLogic()
	{
		rng = new Random();
		Obstacles = new List<Entity>();
		movableBlocks = new HashSet<MovingBlock>();
		Ball = new PlayerBall(0, BallSize);
	}

	public void Initialize(double width, double height)
	{
		Obstacles.Clear();
		movableBlocks.Clear();
		Score = 0;
		BaseLine = 0;
		topBlockYPos = 0;
		rowNumber = 0;

		bounds = (width, height);
		rowCount = (int)(height / BlockSize);
		detector = new CollisionDetector(width);

		GenerateObstacles();
		Ball.MoveTo(width / 2, height / 2);
		Ball.Stop();
		Ball.ResetJumps();
		Ball.Stand = null;

		IsStarted = true;
	}

	private void GenerateObstacles()
	{
		for (; topBlockYPos <= bounds.Item2; topBlockYPos += BlocksInterval)
		{
			var obstaclesRow = CreateObstacleRow(topBlockYPos);
			Obstacles.AddRange(obstaclesRow);
		}

		topBlockYPos -= BlocksInterval;
	}

	private List<Entity> CreateObstacleRow(double yPos)
	{
		// TODO: Create ObstacleGenerator
		const int MaxOneSize = 3;
		const int Margin = BallSize;

		int fullSize = (int)bounds.Item1 / (BlockSize + Margin) + 1;
		var createdEntities = new List<Entity>();
		bool hasGap = false;
		int rowOffset = (++rowNumber % 2 == 0) ? Margin : 0;

		for (int size = 0; size < fullSize;)
		{
			int oneSize = rng.Next(1, MaxOneSize);

			if (rng.NextDouble() > 0.4)
			{
				double xPos = size * (BlockSize + Margin) + rowOffset;
				var newEntity = CreateBlock(xPos, yPos, BlockSize * oneSize, BlockSize);
				createdEntities.Add(newEntity);
			}
			else
			{
				hasGap = true;
			}

			size += oneSize;
		}

		if (!hasGap)
		{
			createdEntities.RemoveAt(rng.Next(createdEntities.Count));
		}

		if ((createdEntities.Count <= 0) ||
		   (createdEntities.Count == 1 && rng.NextDouble() > 0.5))
		{
			double maxX = bounds.Item1 - BlockSize;
			double xPos = rng.NextDouble() * maxX;
			var newMoving = new MovingBlock(xPos, yPos, BlockSize);
			movableBlocks.Add(newMoving);
			createdEntities.Clear();
			createdEntities.Add(newMoving);
		}

		return createdEntities;
	}

	public Block CreateBlock(double x, double y, double width, double height)
	{
		double val = rng.NextDouble();

		if (val < 0.9)
		{
			return new Block(x, y, width, height);
		}
		else
		{
			return new BouncyBlock(x, y, BlockSize);
		}
	}

	public void Interact(double dx, double dy)
	{
		if (Ball.CanJump)
		{
			Ball.Stand = null;
			Ball.Accelerate(dx / 10.0f, 15.0f);
			Ball.Jumped();
		}
	}

	public void Update()
	{
		tick++;

		RaiseBaseLine();
		MoveObstacles();
		MoveBall();

		HandleCollisions();
	}

	private void RaiseBaseLine()
	{
		const int JumpOffset = 200;

		double topLine = BaseLine + bounds.Item2;
		double jumpLine = topLine - JumpOffset;

		bool needRaise = Ball.Y > jumpLine && Ball.Jumping;

		BaseLine += needRaise ? Ball.Velocity.Y : 1.0;

		if (needRaise)
		{
			Score++;
		}

		if (topLine - topBlockYPos > BlocksInterval)
		{
			topBlockYPos = topLine;
			var newRow = CreateObstacleRow(topLine);
			Obstacles.AddRange(newRow);

			foreach (var newBlock in newRow)
			{
				ObjectCreated?.Invoke(this, newBlock);
			}
		}
	}

	private void MoveObstacles()
	{
		foreach (var movingBlock in movableBlocks)
		{
			movingBlock.Move();
		}

		foreach (var block in Obstacles)
		{
			if (block.Y + block.Height < BaseLine)
			{
				Obstacles.Remove(block);
				if (block is MovingBlock movable)
				{
					movableBlocks.Remove(movable);
				}

				ObjectRemoved?.Invoke(this, block);
			}
		}
	}

	private void MoveBall()
	{
		if (Stand == null)
		{
			double friction = -Math.Sign(Ball.Velocity.X) * 0.2;
			Ball.Accelerate(friction, -1.0);
		}
		else if (!Ball.Jumping)
		{
			Ball.Stop();

			if (Stand is MovingBlock movable)
			{
				Ball.MoveBy(movable.Velocity.X, 0.0);
			}
		}

		Ball.Move();
	}

	private void HandleCollisions()
	{
		// Movable block collisions
		foreach (var movingBlock in movableBlocks)
		{
			movingBlock.Bump = detector.CheckCollision(movingBlock, none);
		}

		// Update block states
		foreach (var block in Obstacles)
		{
			block.Update();
		}

		// Ball collisions
		Ball.Bump = detector.CheckCollision(Ball, Obstacles);
		Ball.Update();

		//Stand = Ball.Stand;

		// Game Over condition
		if (Ball.Y + Ball.Height < BaseLine)
		{
			GameOver?.Invoke(this, EventArgs.Empty);
		}
	}
}
