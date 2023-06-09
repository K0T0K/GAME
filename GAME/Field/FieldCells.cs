using System;
using Game1.Characters.Interfaces;
using Microsoft.Xna.Framework;

namespace GAME.Field;

public class FieldCell
{
    public FieldCell(Vector2 coordinates, Point fieldCoordinates)
    {
        Coordinates = coordinates;
        FieldCoordinates = fieldCoordinates;
    }

    public ICharacter CurrentCharacter { get; set; }
    public Vector2 Coordinates { get; set; }
    public Point FieldCoordinates { get; set; }

    public Vector2 GetCharacterPosition(ICharacter character)
    {
        var dx = character.GetCurrentImage().Width * character.ImageScaleOnField / 4;
        var dy = character.GetCurrentImage().Height * character.ImageScaleOnField / 4;

        return new Vector2(Coordinates.X - dx, Coordinates.Y - dy);
    }

    public bool IsIntersected(Vector2 location)
    {
        return location.X - Coordinates.X <= 60 && location.X - Coordinates.X > 0 
                                                && location.Y - Coordinates.Y <= 60 && location.Y - Coordinates.Y > 0;
    }
}
    