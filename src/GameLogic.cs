using Godot;
using System;

public static class GameLogic {

	public static readonly Vector2 Gravity = (Vector2)ProjectSettings.GetSetting("physics/2d/default_gravity_vector");
    public static readonly Random Random = new Random();
}

public static class Extensions
{
	public static double NextBetween(this Random random, float min, float max)
	{
		return random.NextDouble() * (max - min) + min;
    }

    public static Vector2 NextPointInside(this Random random, Vector2 extents, Vector2 position = default)
    {
        return new Vector2((float) random.NextBetween(-extents.x, extents.x), 
            (float) random.NextBetween(-extents.y, extents.y)) + position;
    }
}