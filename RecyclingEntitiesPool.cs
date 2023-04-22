using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace BouncyBall;

/// <summary>
/// Holds game entities which can be reused
/// </summary>
public class RecyclingEntitiesPool 
{
	private const int BrickTexturesCount = 3;
	
	private readonly Dictionary<Entity, Frame> entityElems = new();

	private readonly HashSet<Entity> created = new();

	private readonly HashSet<Entity> removed = new();

	private readonly ImageSource[] brickImages;
	
	public RecyclingEntitiesPool()
	{
		brickImages = Enumerable.Range(1, BrickTexturesCount)
			.Select(i =>
			{
				string resName = $"BouncyBall.resources.brick{i}.png";
				return ImageSource.FromResource(resName);
			})
			.ToArray();
	}
	
	public void Reset()
	{
		entityElems.Clear();
		created.Clear();
		removed.Clear();
	}
	
	public Frame GetFrame(Entity entity) => entityElems[entity];
	
	public void EntityAdded(Entity entity) => created.Add(entity);
	
	public void EntityRemoved(Entity entity) => removed.Add(entity);
	
	public Dictionary<Entity, Frame> GetAllEntities() => entityElems
		.Where(kv => !removed.Contains(kv.Key))
		.ToDictionary(kv => kv.Key, kv => kv.Value);
	
	public Frame CreateOrRecycleFrame(Entity block, out bool recycled)
	{
		var recycledBlock = removed.FirstOrDefault(b => b.GetType() == block.GetType());
		recycled = recycledBlock != null;
		
		Frame blockFrame;
		
		if (recycled)
		{
			removed.Remove(recycledBlock);
			blockFrame = entityElems[recycledBlock];
			entityElems.Remove(recycledBlock);
		}
		else
		{
			blockFrame = new Frame()
			{
				BackgroundColor = Colors.Gray,
				BorderColor = Colors.Black,
				Padding = new Thickness(1),
				Content = new Image()
				{
					Aspect = Aspect.Fill,
					Source = block switch
					{
						MovingBlock => brickImages[1],
						BouncyBlock => brickImages[2],
						_ => brickImages[0]
					}
				}
			};
		}
		
		entityElems.Add(block, blockFrame);
		return blockFrame;
	}

	public void HandleBlocks(Action<IEnumerable<Entity>> createHandler, Action<Frame> removeHandler)
	{
		const int MaxEntities = 20;
		
		// Place new blocks
		if (created.Count > 0)
		{
			createHandler(created);
			created.Clear();
		}
		
		// Remove block
		if (removed.Count >= MaxEntities)
		{
			Console.WriteLine($"Clear at {Environment.TickCount}");
		
			foreach (var removedBlock in removed)
			{
				var blockFrame = entityElems[removedBlock];
				removeHandler(blockFrame);
				entityElems.Remove(removedBlock);
	 		}

			removed.Clear();
		}
	}
}
