﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall.Game;

/// <summary>
/// Detects collisions of entities
/// </summary>
public class CollisionDetector
{
	private readonly double width;

	public CollisionDetector(double width) => this.width = width;

	public bool CheckCollides(Entity one, Entity another)
	{
		return (one.X + one.Width >= another.X) &&
			   (another.X + another.Width >= one.X) &&
			   (one.Y + one.Height >= another.Y) &&
			   (another.Y + another.Height >= one.Y);
	}

	public Collision CheckCollision(Entity one, IList<Entity> others)
	{
		foreach (var ent in others)
		{
			if (CheckCollides(one, ent))
			{
				return GetCollisionDetails(one, ent);
			}
		}

		if (one.X < 0)
		{
			return new Collision { Side = CollisionSide.Right };
		}
		else if (one.X + one.Width > width)
		{
			return new Collision { Side = CollisionSide.Left, RightBound = width };
		}

		return null;
	}

	private Collision GetCollisionDetails(Entity one, Entity another)
	{
		var collision = new Collision { Block = another };

		if ((one.Velocity.X > 0.0) && (one.X + one.Width > another.X))
		{
			collision.Side = CollisionSide.Left;
		}
		else if ((one.Velocity.X < 0.0) && (one.X < another.X + another.Width))
		{
			collision.Side = CollisionSide.Right;
		}

		var side = collision.Side;

		if (side == CollisionSide.Top || side == CollisionSide.Bottom)
		{
			collision.Side = side;
		}
		else
		{
			var vSide = one.Jumping ? CollisionSide.Bottom : CollisionSide.Top;
			var testEntity = new Block(one.X, one.Y - one.Velocity.Y, one.Width, one.Height);

			collision.Side = !CheckCollides(testEntity, another)
				? vSide
				: side;
		}

		return collision;
	}
}
