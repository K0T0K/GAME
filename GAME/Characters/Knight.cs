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
    public static int CurrentFrame;
    private static List<Texture2D> _idleImages = new();
    private static List<Texture2D> _walkImages = new();
    private int _animationSpeed = 6;
    private Size _imageSize;
    private ICharacter _characterImplementation;

    public Knight(GraphicsDevice graphicsDevice, int price, Player player, Vector2 initialPosition = new(), float scaleOnField = 1, float scaleOnStore = 1)
    {
        for (var i = 0; i < 10; i++)
        {
            _idleImages.Add(Texture2D.FromFile(graphicsDevice, @$"Images\Idle\Knight_01__IDLE_00{i}.png"));
            _walkImages.Add(Texture2D.FromFile(graphicsDevice, @$"Images\Walk\Knight_01__WALK_00{i}.png"));
        }

        Player = player;
        Price = price;
        _imageSize = new Size(_walkImages[0].Width, _walkImages[0].Height);
        ImageScaleOnStore = scaleOnStore;
        ImageScaleOnField = scaleOnField;
        ImageLocation = initialPosition;
    }

    public Vector2 ImageLocation { get; set; }
    
    public CharacterState State { get; set; }
    
    public float ImageScaleOnStore { get; set; }
    public float ImageScaleOnField { get; set; }
    public int Price { get; set; }
    public Player Player { get; set; }
    public Vector2 GetPositionForCenterDrawingOnStore(Vector2 mousePositon)
    {
        return new Vector2(mousePositon.X - _imageSize.Width * ImageScaleOnStore / 2,
            mousePositon.Y - _imageSize.Height * ImageScaleOnStore/ 2);
    }
    public Vector2 GetPositionForCenterDrawingOnField(Vector2 mousePosition)
    {
        return new Vector2(x: mousePosition.X - _imageSize.Width * ImageScaleOnField / 2,
            y: mousePosition.Y - _imageSize.Height * ImageScaleOnField / 2);
    }
    
    public bool IsClicked(MouseState mousePoint)
    {
        return mousePoint.X - ImageLocation.X <= _imageSize.Width * ImageScaleOnStore
               && mousePoint.X - ImageLocation.X >= 0
               && mousePoint.Y - ImageLocation.Y <= _imageSize.Height * ImageScaleOnStore
               && mousePoint.Y - ImageLocation.Y >= 0;
    }
    

    public Texture2D GetCurrentImage()
    {
        if (CurrentFrame / _animationSpeed >= _idleImages.Count)
        {
            CurrentFrame = 0;
        }

        if (State == CharacterState.Walk)
            return _walkImages[CurrentFrame / _animationSpeed];

        return _idleImages[CurrentFrame / _animationSpeed];
    }

    public void Move(Vector2 delta)
    {
        ImageLocation = new Vector2(ImageLocation.X + delta.X, ImageLocation.Y + delta.Y);
        State = CharacterState.Walk;
    }

    public override bool Equals(object obj)
    {
        if (obj is not ICharacter character)
        {
            return false;
        }

        return character.ImageLocation == ImageLocation;
    }
}