using System;
using Game1.Characters.Interfaces;
using Microsoft.Xna.Framework;

namespace GAME.Field;

public class FieldCell
{
    public FieldCell(Vector2 coordinates)
    {
        Coordinates = coordinates;
    }

    public ICharacter CurrentCharacter { get; set; }
    public Vector2 Coordinates { get; set; }

    public Vector2 GetCharacterPosition(ICharacter character)
    {
        var dx = character.GetCurrentImage().Width * character.ImageScaleOnField / 4;
        var dy = character.GetCurrentImage().Height * character.ImageScaleOnField / 4;

        return new Vector2(Coordinates.X - dx, Coordinates.Y - dy);
    }

    public bool IsIntersected(Vector2 location)
    {
        return location.X - Coordinates.X <= 15 && location.X - Coordinates.X > -40 
               && location.Y - Coordinates.Y <= 15 && location.Y - Coordinates.Y > -40;
    }
}
    