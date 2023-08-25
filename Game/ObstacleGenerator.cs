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
            
    private readonly Random rng = new();
    
    private int rowNumber = 0;
    
    private double width;
    
    public ObstacleGenerator(double width) => this.width = width;
    
    /// <summary>
    /// Create line of obstacles
    /// </summary>
	public List<Entity> CreateObstacleRow(double yPos, out bool hasMovable)
	{
		const int MaxOneSize = 3;
		const int Margin = GameLogic.BallSize;

		int fullSize = (int)width / (BlockSize + Margin) + 1;
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
}
