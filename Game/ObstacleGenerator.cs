using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall.Game;

/// <summary>
/// Generates game obstacles
/// </summary>
public class ObstacleGenerator 
{
    
    private const int BlockSize = GameLogic.BlockSize;
    
    private const int Margin = GameLogic.BallSize;
            
    private readonly Random rng = new();
    
    private readonly double width;
    
    private readonly int fullSize;
    
    private readonly double randomness;
    
    private int rowNumber = 0;
    
    private double yPos;
    
    private double phi;
    
    public ObstacleGenerator(double width, int randomLevel)
    {
        this.width = width;
        randomness = randomLevel / 10.0;
        fullSize = (int)width / (BlockSize + Margin) + 1;
        phi = rng.NextDouble() * Math.PI;
    }
    
    /// <summary>
    /// Create line of obstacles
    /// </summary>
	public List<Entity> CreateObstacleRow(double yPos, out bool hasMovable)
	{
        this.yPos = yPos;
        
        double density = (rng.NextDouble() > randomness)
            ? Math.Abs(Math.Sin(rowNumber / 10.0 + phi))
            : 0.5;
            
        density = (density < 0.1) ? 0.1 : density;
            
        return GenerateObstacles(density, out hasMovable);
    }
    
    private List<Entity> GenerateObstacles(double density, out bool hasMovable)
    {
        const int MaxOneSize = 3;

		var createdEntities = new List<Entity>();
		bool hasGap = false;
		int rowOffset = (++rowNumber % 2 == 0) ? Margin : 0;

		for (int size = 0; size < fullSize;)
		{
			int oneSize = rng.Next(1, MaxOneSize);

			if (rng.NextDouble() < density)
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
		   (createdEntities.Count == 1 && rng.NextDouble() < 0.5))
		{
			double maxX = width - BlockSize;
			double xPos = rng.NextDouble() * maxX;
			var newMoving = new MovingBlock(xPos, yPos, BlockSize);
			hasMovable = true;
			createdEntities.Clear();
			createdEntities.Add(newMoving);
		}
        else
        {
            hasMovable = false;
        }

		return createdEntities;
	}

	private Block CreateBlock(double x, double y, double width, double height)
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
}
