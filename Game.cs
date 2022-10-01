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
    	
    	public bool IsStarted { get; private set; }
    	
    	public double BaseLine { get; private set; }
    	
    	public Entity Ball { get; }
    	
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
        	
        	GenerateObstacles();
        	
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
        
        public void Update()
        {
			MoveBall();
        	tick++;
        	BaseLine += 1.0;
        	
        	foreach(var block in Obstacles)
        	{
        		if(block.Y < BaseLine)
        		{
        			ObjectRemoved?.Invoke(this, block);
        			Obstacles.Remove(block);
        			
        			var randomObstacle = CreateObstacle(true);
        			Obstacles.Add(randomObstacle);
        			ObjectCreated?.Invoke(this, randomObstacle);
        		}
        	}
        }
        
        private void MoveBall()
        {
        	var (w, h) = bounds;
        	double hw = w / 2;
			double hh = h / 2;
			
			double t = tick / 10.0;
			double x = Math.Cos(t * 0.5) * 200 + hw;
			double y = Math.Sin(t) * 200 + hh;
			
			Ball.MoveTo(x, y);
        }
    }
}