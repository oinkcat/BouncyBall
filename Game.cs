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
    	
    	private (double, double) bounds;
    	
    	private int rowCount;
    	
    	private Random rng;
    	
    	private int tick;
    	
    	private CollisionDetector detector;
    	
    	public bool IsStarted { get; private set; }
    	
    	public double BaseLine { get; private set; }
    	
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
        	const int NumObstaclesMin = 5;
        	const int NumObstaclesMax = 10;
        	
        	var numObstacles = rng.Next(NumObstaclesMin, NumObstaclesMax);
        	
        	for(int i = 0; i < numObstacles; i++)
        	{
        		Obstacles.Add(CreateObstacle());
        	}
        }
        
        private Entity CreateObstacle(bool onTop = false)
        {
        	var (maxX, maxY) = bounds;
        	
        	double x = rng.NextDouble() * maxX;
        	
        	double y = onTop
        		? maxY + BaseLine + BlockSize
        		: rng.Next(rowCount) * BlockSize;
        	
        	double width = (rng.NextDouble() * 3 + 1) * BlockSize;
        	return new Entity(x, y, width, BlockSize);
        }
        
        public void Interact(double dx, double dy)
        {
        	Stand = null;
        	Ball.Accelerate(dx / 10.0f, 15.0f);
        }
        
        public void Update()
        {
			MoveBall();
        	tick++;
        	
        	double topLine = BaseLine + bounds.Item2 - 50;
        	BaseLine += (Ball.Y > topLine && Ball.Jumping)
        		? Ball.Velocity.Y
        		: 1.0;
        	
        	MoveObstacles();
        	
        	var collision = detector.CheckCollision(Ball, Obstacles);
			
        	if(collision != null)
        	{
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
        			Ball.MoveOnEntity(Stand);
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
        			ObjectRemoved?.Invoke(this, block);
        			Obstacles.Remove(block);
        			
        			var randomObstacle = CreateObstacle(true);
        			Obstacles.Add(randomObstacle);
        			ObjectCreated?.Invoke(this, randomObstacle);
        		}
        	}
        }
    }
}