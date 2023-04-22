using System;
using System.Collections.Generic;
using System.Drawing;
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
    private Size _imageSize;
    private ICharacter _characterImplementation;

    public Knight(GraphicsDevice graphicsDevice,int price, Vector2 initialPosition = new Vector2(), float scale = 1)
    {
        for (var i = 0; i < 10; i++)
        {
            _idleImages.Add(Texture2D.FromFile(graphicsDevice, @$"Images\Idle\Knight_01__IDLE_00{i}.png"));
            _walkImages.Add(Texture2D.FromFile(graphicsDevice, @$"Images\Walk\Knight_01__WALK_00{i}.png"));
        }

        Price = price;
        _imageSize = new Size(_walkImages[0].Width, _walkImages[0].Height);
        ImageScale = scale;
        ImageLocation = initialPosition;
    }

    public Vector2 ImageLocation { get; set; }
    
    public CharacterState State { get; set; }
    
    public float ImageScale { get; set; }

    public int Price { get; set; }
    public Vector2 GetPositionForCenterDrawing(Vector2 mousePositon)
    {
        return new Vector2(mousePositon.X - _imageSize.Width * ImageScale / 2,
            mousePositon.Y - _imageSize.Height * ImageScale / 2);
    }
    public bool IsClicked(MouseState mousePoint)
    {
        return mousePoint.X - ImageLocation.X <= _imageSize.Width * ImageScale
               && mousePoint.X - ImageLocation.X >= 0
               && mousePoint.Y - ImageLocation.Y <= _imageSize.Height * ImageScale
               && mousePoint.Y - ImageLocation.Y >= 0;
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
        ImageLocation = new Vector2(ImageLocation.X + delta.X, ImageLocation.Y + delta.Y);
        State = CharacterState.Walk;
    }
}