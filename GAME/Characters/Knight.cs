using System.Collections.Generic;
using System;
using Game1.Characters.Enums;
using Game1.Characters.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1.Characters;

public class Knight : ICharacter
{
    private static List<Texture2D> _idleImages = new();
    private static List<Texture2D> _walkImages = new();
    private int _currentFrame = 0;
    private int _animationSpeed = 6;

    public Knight(GraphicsDevice graphicsDevice, Vector2 initialPosition = new Vector2())
    {
        for (var i = 0; i < 10; i++)
        {
            _idleImages.Add(Texture2D.FromFile(graphicsDevice, @$"Images\Idle\Knight_01__IDLE_00{i}.png"));
            _walkImages.Add(Texture2D.FromFile(graphicsDevice, @$"Images\Walk\Knight_01__WALK_00{i}.png"));
        }

        Location = initialPosition;
    }

    public Vector2 Location { get; set; }
    
    public CharacterState State { get; set; }
    
    public bool IsClicked(MouseState mousePoint)
    {
        return Math.Abs (Location.X - mousePoint.X) 
            <= 50 && Math.Abs (Location.Y - mousePoint.Y) <= 50;
    }
    

    public Texture2D GetCurrentImage()
    {
        _currentFrame++;

        if (_currentFrame / _animationSpeed >= _idleImages.Count)
        {
            _currentFrame = 0;
        }

        if (State == CharacterState.Walk)
            return _walkImages[_currentFrame / _animationSpeed];

        return _idleImages[_currentFrame / _animationSpeed];
    }

    public void Move(Vector2 delta)
    {
        Location = new Vector2(Location.X + delta.X, Location.Y + delta.Y);
        State = CharacterState.Walk;
    }
}