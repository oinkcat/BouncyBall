using System;
using System.Numerics;
using System.Collections.Generic;

namespace BouncyBall.Game;

/// <summary>
/// Base game entity
/// </summary>
public abstract class Entity
{
	private const float MaxVelocity = 10;

	public Collision Bump { get; set; }

	public Vector2 Position { get; private set; }

	public Vector2 Velocity { get; private set; }

	public int Width { get; }

	public int Height { get; }

	public double X => (double)Position.X;

	public double Y => (double)Position.Y;

	public bool Jumping => Velocity.Y > 0.0f;

	public Entity(double x, double y, double width, double height)
	{
		Position = new Vector2((float)x, (float)y);
		Velocity = new Vector2(0);

		Width = (int)width;
		Height = (int)height;
	}

	public void MoveTo(double x, double y)
	{
		Position = new Vector2((float)x, (float)y);
	}

	public void MoveBy(double dx, double dy)
	{
		Position += new Vector2((float)dx, (float)dy);
	}

	public void MoveOnEntity(Entity another, CollisionSide side = CollisionSide.Top)
	{
		float newX = side switch
		{
			CollisionSide.Left => (float)another.X - Width,
			CollisionSide.Right => (float)another.X + another.Width,
			_ => Position.X
		};

		float newY = side switch
		{
			CollisionSide.Top => (float)another.Y + another.Height,
			CollisionSide.Bottom => (float)another.Y - Height,
			_ => Position.Y
		};

		Position = new Vector2(newX, newY);
	}

	public void Accelerate(double ax, double ay)
	{
		var newVelocity = Velocity + new Vector2((float)ax, (float)ay);

		float vx = Math.Clamp(newVelocity.X, -MaxVelocity, MaxVelocity);
		float vy = Math.Clamp(newVelocity.Y, -MaxVelocity, MaxVelocity * 2);
		Velocity = new Vector2(vx, vy);
	}

	public void Move()
	{
		Position += Velocity;
	}

	public void Stop()
	{
		Velocity = new Vector2(0.0f);
	}

	public abstract void Update();
}
