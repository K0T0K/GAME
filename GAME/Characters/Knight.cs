using System;
using System.Collections.Generic;
using System.Drawing;
using GAME.Field;
using Game1.Characters.Enums;
using Game1.Characters.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Point = Microsoft.Xna.Framework.Point;

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
        Health = 3;
        FuturePosition = ImageLocation;
    }

    public Vector2 ImageLocation { get; set; }
    
    public CharacterState State { get; set; }
    
    public float ImageScaleOnStore { get; set; }
    public float ImageScaleOnField { get; set; }
    public int Price { get; set; }
    public Player Player { get; set; }
    public int Health { get; set; }
    public Point FieldCoordinates { get; set; }
    
    public ICharacter GetCopy(GraphicsDevice graphicsDevice)
    {
        return new Knight(graphicsDevice, Price, Player, ImageLocation, ImageScaleOnField, ImageScaleOnStore);
    }

    public Vector2 FuturePosition { get; set; }
    public void UpdatePosition()
    {
        if (ImageLocation != FuturePosition)
        {
            var dx = (FuturePosition.X - ImageLocation.X) / 3;
            var dy = (FuturePosition.Y - ImageLocation.Y) / 3;
            ImageLocation += new Vector2(dx, dy);
        }
    }

    public void SetNewPosition(Vector2 newPosition)
    {
        ImageLocation = FuturePosition = newPosition;
    }

    public void Move(FieldCell[,] field)
    {
        var enemy = FieldHelper.GetMostAttractiveEnemy(FieldCoordinates, field);
        if (enemy == null)
        {
            return;
        }

        var freeWays = FieldHelper.GetFreeWays(FieldCoordinates, field);

        if (freeWays.Length == 0)
        {
            return;
        }

        if (Math.Abs(FieldCoordinates.X - enemy.FieldCoordinates.X) == 1 && Math.Abs(FieldCoordinates.Y - enemy.FieldCoordinates.Y) == 0
            || Math.Abs(FieldCoordinates.X - enemy.FieldCoordinates.X) == 0 && Math.Abs(FieldCoordinates.Y - enemy.FieldCoordinates.Y) == 1)
        {
            enemy.Health--;
            return;
        }

        foreach (var freeWay in freeWays)
        {
            if ((freeWay.X - FieldCoordinates.X) * (enemy.FieldCoordinates.X - FieldCoordinates.X) > 0 
                || (freeWay.Y - FieldCoordinates.Y) * (enemy.FieldCoordinates.Y - FieldCoordinates.Y) > 0)
            {
                field[freeWay.X, freeWay.Y].CurrentCharacter = field[FieldCoordinates.X, FieldCoordinates.Y].CurrentCharacter;
                field[FieldCoordinates.X, FieldCoordinates.Y].CurrentCharacter = null;
                FuturePosition = field[freeWay.X, freeWay.Y]
                    .GetCharacterPosition(field[freeWay.X, freeWay.Y].CurrentCharacter);
                FieldCoordinates = freeWay;
                return;
            }
        }
    }

    public Vector2 GetPositionForCenterDrawingOnStore(Vector2 mousePosition)
    {
        return new Vector2(mousePosition.X - _imageSize.Width * ImageScaleOnStore / 2,
            mousePosition.Y - _imageSize.Height * ImageScaleOnStore / 2);
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

        return Math.Abs(character.ImageLocation.X - ImageLocation.X) < 10 && Math.Abs(character.ImageLocation.Y - ImageLocation.Y) < 10;
    }
}