﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall.Game;

public enum CollisionSide
{
	Uncertain,
	Left,
	Right,
	Top,
	Bottom
}

/// <summary>
/// Ball collision info
/// </summary>
public class Collision
{
	public Entity Block { get; set; }

	public CollisionSide Side { get; set; }

	public double? RightBound { get; set; }

	public bool IsBoundary => Block == null;

	public bool IsSide => (Side == CollisionSide.Left) ||
						  (Side == CollisionSide.Right);
}
