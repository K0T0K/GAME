using System;
using Microsoft.Xna.Framework;

namespace Game1.Characters;

public class Computer
{
    private Random random = new();

    public Vector2? PlaceKnight()
    {
        // if (random.Next(0, 1000) < 10)
        //{
        return new Vector2(random.Next(22, 928), random.Next(100, 576));
        //}

        return null;
    }
}