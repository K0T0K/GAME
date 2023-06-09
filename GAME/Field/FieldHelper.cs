using System.Collections.Generic;
using Game1.Characters.Interfaces;
using Microsoft.Xna.Framework;

namespace GAME.Field;

public static class FieldHelper
{
    public static ICharacter GetMostAttractiveEnemy(Point start, FieldCell[,] field)
    {
        var queue = new Queue<Point>();
        queue.Enqueue(start);
        var visited = new HashSet<Point>();
        var player = field[start.X, start.Y].CurrentCharacter.Player;

        while (queue.Count > 0)
        {
            var nextPoint = queue.Dequeue();
            if (field[nextPoint.X, nextPoint.Y].CurrentCharacter != null &&
                field[nextPoint.X, nextPoint.Y].CurrentCharacter.Player != player)
            {
                return field[nextPoint.X, nextPoint.Y].CurrentCharacter;
            }

            visited.Add(nextPoint);
            
            AddNeighbours(nextPoint, queue, field, visited);
        }

        return null;
    }

    public static Point[] GetFreeWays(Point start, FieldCell[,] field)
    {
        var freeWays = new List<Point>();

        for (var dx = -1; dx < 2; dx++)
        {
            for (var dy = -1; dy < 2; dy++)
            {
                if (dx != 0 && dy != 0)
                {
                    continue;
                }

                var newPoint = new Point(start.X + dx, start.Y + dy);
                if (!IsInField(newPoint, field) || field[newPoint.X, newPoint.Y].CurrentCharacter != null)
                {
                    continue;
                }
                
                freeWays.Add(newPoint);
            }
        }

        return freeWays.ToArray();
    }

    private static void AddNeighbours(Point start, Queue<Point> queue, FieldCell[,] field, HashSet<Point> visited)
    {
        for (var dx = -1; dx < 2; dx++)
        {
            for (var dy = -1; dy < 2; dy++)
            {
                if (dx != 0 && dy != 0)
                {
                    continue;
                }

                var newPoint = new Point(start.X + dx, start.Y + dy);
                if (visited.Contains(newPoint) || !IsInField(newPoint, field))
                {
                    continue;
                }
                
                queue.Enqueue(newPoint);
            }
        }
    }

    private static bool IsInField(Point point, FieldCell[,] field)
    {
        return point.Y >= 0
               && point.Y < field.GetLength(1)
               && point.X >= 0
               && point.X < field.GetLength(0);
    }
}
