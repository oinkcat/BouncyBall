using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall.Game;

/// <summary>
/// Game logic
/// </summary>
public class GameLogic
{
	public const int BlockSize = 60;

	public const int BallSize = 30;

	private const int BlocksInterval = 150;

	private readonly static List<Entity> none = new List<Entity>();

	private (double, double) bounds;

	private readonly Random rng;
    
    private int randomLevel;

	private int tick;
 
	private int rowCount;
 
    private double topBlockYPos;

	private readonly HashSet<MovingBlock> movableBlocks;
    
    private ObstacleGenerator blocksGenerator;

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

		bounds = (width, height);
        this.randomLevel = Settings.Instance.RandomnessLevel;
		rowCount = (int)(height / BlockSize);
		detector = new CollisionDetector(width);
        blocksGenerator = new ObstacleGenerator(width, randomLevel);

		GenerateObstacles();
        Ball.BounceFloor = Settings.Instance.ExtraBouncy;
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
			_ = CreateBlocksRow();
		}

		topBlockYPos -= BlocksInterval;
	}
    
    private List<Entity> CreateBlocksRow()
    {
        var obstaclesRow = blocksGenerator.CreateObstacleRow(topBlockYPos, out bool movable);
		Obstacles.AddRange(obstaclesRow);
        
        if(movable)
        {
            movableBlocks.Add(obstaclesRow.First() as MovingBlock);
        }
        
        return obstaclesRow;
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
			var newRow = CreateBlocksRow();

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
			Ball.ApplyFriction();
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
