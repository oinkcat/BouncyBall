using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall
{
	/// <summary>
	/// Game logic
	/// </summary>
    public class Game 
    {
    	private const int BlockSize = 60;
    	
		private const int BallSize = 30;
		
		private const int BlocksInterval = 150;
    	
    	private (double, double) bounds;
    	
    	private int rowCount;
    	
    	private Random rng;
    	
    	private int tick;
    	
    	private double topBlockYPos;
    	
    	private CollisionDetector detector;
    	
    	public bool IsStarted { get; private set; }
    	
    	public double BaseLine { get; private set; }
    	
    	public int Score { get; private set; }
    	
    	public Entity Ball { get; }
    	
    	public Entity Stand { get; private set; }
    	
    	public List<Entity> Obstacles { get; }
    	
    	public EventHandler<Entity> ObjectCreated;
    	
    	public EventHandler<Entity> ObjectRemoved;
    	
    	public Game()
    	{
    		Obstacles = new List<Entity>();
        	Ball = new Entity(0, -BallSize, BallSize, BallSize);
    	}
    	
        public void Initialize(double width, double height)
        {
        	bounds = (width, height);
        	rowCount = (int)(height / BlockSize);
        	rng = new Random();
        	detector = new CollisionDetector(width);
        	
        	GenerateObstacles();
        	Ball.MoveTo(width / 2, height / 2);
        	
        	IsStarted = true;
        }
        
        private void GenerateObstacles()
        {
        	const int InitialHeight = 400;
        	
        	for(;topBlockYPos < InitialHeight; topBlockYPos += BlocksInterval)
        	{
        		var obstaclesRow = CreateObstacleRow(topBlockYPos);
        		Obstacles.AddRange(obstaclesRow);
        	}
        }
        
        private List<Entity> CreateObstacleRow(double yPos)
        {
        	const int MaxOneSize = 3;
        	const int Margin = BallSize;
        	
        	int fullSize = (int)bounds.Item1 / (BlockSize + Margin) + 1;
        	var createdEntities = new List<Entity>();
        	bool hasGap = false;
        	
        	for(int size = 0; size < fullSize;)
        	{
        		int oneSize = rng.Next(1, MaxOneSize);
        		
        		if(rng.NextDouble() > 0.33)
        		{
        			double xPos = size * (BlockSize + Margin);
        			var newEntity = new Entity(xPos, yPos, BlockSize * oneSize, BlockSize);
        			createdEntities.Add(newEntity);
        		}
        		else
        		{
        			hasGap = true;
        		}
        		
        		size += oneSize;
        	}
        	
        	if(!hasGap)
        	{
        		createdEntities.RemoveAt(rng.Next(createdEntities.Count));
        	}
        	
        	return createdEntities;
        }
        
        public void Interact(double dx, double dy)
        {
        	Stand = null;
        	Ball.Accelerate(dx / 10.0f, 15.0f);
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
        	
        	if(needRaise)
        	{
        		Score++;
        	}
        	
        	if(topLine - topBlockYPos > BlocksInterval)
        	{
        		topBlockYPos = topLine;
        		var newRow = CreateObstacleRow(topLine);
        		Obstacles.AddRange(newRow);
        		
        		foreach(var newBlock in newRow)
        		{
        			ObjectCreated?.Invoke(this, newBlock);
        		}
        	}
        }
        
        private void MoveBall()
        {
			if(Stand == null)
			{
				double friction = -Math.Sign(Ball.Velocity.X) * 0.2;
				Ball.Accelerate(friction, -1.0);
			}
			else if(!Ball.Jumping)
			{
				Ball.Stop();
			}
			
			Ball.Move();
        }
        
        private void MoveObstacles()
        {
        	foreach(var block in Obstacles)
        	{
        		if(block.Y + block.Height < BaseLine)
        		{
        			Obstacles.Remove(block);
        			ObjectRemoved?.Invoke(this, block);
        		}
        	}
        }
        
        private void HandleCollisions()
        {
        	var collision = detector.CheckCollision(Ball, Obstacles);
			
        	if(collision != null)
        	{
        		if(collision.Block != null)
        		{
        			Ball.MoveOnEntity(collision.Block, collision.Side);
        		}
        		else
        		{
        			double xBySide = (collision.Side == CollisionSide.Left)
        				? bounds.Item1 - Ball.Width
        				: 0;
        			Ball.MoveTo(xBySide, Ball.Y);
        		}
        		
        		if(collision.IsSide)
        		{
        			Ball.Accelerate(-Ball.Velocity.X * 2, 0.0);
        		}
        		else if(collision.Side == CollisionSide.Bottom)
        		{
        			Ball.Accelerate(0.0, -Ball.Velocity.Y * 2);
        		}
        		else if(collision.Side == CollisionSide.Top)
        		{
        			Stand = collision.Block;
        			Ball.Stop();
        		}
        	}
        }
    }
}