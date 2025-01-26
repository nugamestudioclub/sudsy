using Godot;
using System;

public static class GameLogic {

	public static readonly Vector2 Gravity = (Vector2)ProjectSettings.GetSetting("physics/2d/default_gravity_vector");
    public static readonly Random Random = new Random();

    public static string GetLetterGrade(float percentClean, float timeRemaining, float totalTime)
    {
        if (percentClean < .50) return "F";
        if (percentClean < .55) return "D-";
        if (percentClean < .60) return "D";
        if (percentClean < .65) return "D+";
        if (percentClean < .70) return "C-";
        if (percentClean < .75) return "C";
        if (percentClean < .80) return "C+";
        if (percentClean < .85) return "B-";
        if (percentClean < .90) return "B";
        if (percentClean < .95) return "B+";
        if (percentClean < .9999) return "A-";
        float remainingTimeRatio = timeRemaining / totalTime;
        if (remainingTimeRatio < .1) return "A";
        if (remainingTimeRatio < .2) return "A+";
        if (remainingTimeRatio < .3) return "S-";
        if (remainingTimeRatio < .35) return "S";
        if (remainingTimeRatio < .40) return "S+";
        if (remainingTimeRatio < .45) return "S++";
        return "Z";

    }
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