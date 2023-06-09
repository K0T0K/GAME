using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GAME.Controls;
using GAME.Field;
using Game1.Characters;
using Game1.Characters.Enums;
using Game1.Characters.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;


namespace GAME;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private List<ICharacter> _charactersOnField = new();
    private List<ICharacter> _charactersOnStore = new();
    private ICharacter selectedCharacter;
    private SizeF _cellSize = new(90.6F, 68);
    private Vector2 _storeLocation = new(-50, 555);
    private SpriteFont _font;
    private Texture2D _line;
    private Texture2D _redRect;
    private Texture2D _greenRect;
    private Texture2D _fon;
    private FieldCell[,] _field = new FieldCell[10, 7];
    private Computer _computer = new ();
    private Button _spawnButton;
    private ButtonState _previousButtonState;
    private int _framesCounter = 1;
    private int _movesLength = 120;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1080;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        

        var x = 50f;
        var y = 100f;
        for (var i = 0; i < 7; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                _field[j, i] = new FieldCell(new Vector2(x, y), new Point(j, i));
                x += _cellSize.Width * (0.939f + 0.007625f * i);
            }

            y += 68;
            x = 50f - i * _cellSize.Width * (0.007625f * i);
        }
            
        _line = Texture2D.FromFile(GraphicsDevice, @"Images\Pictures\line.png");
        _redRect = Texture2D.FromFile(GraphicsDevice, @"Images\Pictures\red.png");
        _greenRect = Texture2D.FromFile(GraphicsDevice, @"Images\Pictures\green.png");
        _font = Content.Load<SpriteFont>(@"Fonts\File");
        _fon = Texture2D.FromFile(GraphicsDevice, @"Images\Fons\fon.png");
        _spawnButton = new Button("Spawn", new Vector2(970, 20),
            Texture2D.FromFile(GraphicsDevice, @"Images\Pictures\Button.png"), 0.1f);
        
        _charactersOnStore.Add(new Knight(GraphicsDevice, 100, Player.Human, _storeLocation, 0.1f, 0.2f));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        Knight.CurrentFrame++;
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        if (Mouse.GetState().LeftButton == ButtonState.Pressed 
            && Mouse.GetState().LeftButton != _previousButtonState 
            && _spawnButton.IsPressed(Mouse.GetState().Position.ToVector2())
            && _charactersOnField.Count < 16)
        {
            var computerKnightLocation = _computer.PlaceKnight();
            var needPlace = _charactersOnField.Count;
            var placed = 0;
            
            while (placed < needPlace)
            {
                if (computerKnightLocation.HasValue)
                {
                    var enemyKnight = new Knight(GraphicsDevice, 0, Player.Computer,
                        computerKnightLocation.Value, 0.1f, 0.2f);

                    var intersectedCell = GetIntersectedCell(computerKnightLocation.Value);

                    if (intersectedCell != null && intersectedCell.CurrentCharacter == null)
                    {
                        enemyKnight.ImageLocation = intersectedCell.GetCharacterPosition(enemyKnight);
                        intersectedCell.CurrentCharacter = enemyKnight;
                        _charactersOnField.Add(enemyKnight);
                        enemyKnight.FieldCoordinates = intersectedCell.FieldCoordinates;
                        placed++;
                    }
                }

                computerKnightLocation = _computer.PlaceKnight();
            }
        }
        
        foreach (var character in _charactersOnField)
        {
            if (_framesCounter % _movesLength == 0)
            {
                character.Move(_field);
            }
            
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && character.IsClicked(Mouse.GetState())
                 && selectedCharacter == null && character.Player == Player.Human)
            {
                var intersectedCell = GetIntersectedCell(Mouse.GetState().Position.ToVector2());
                
                if (intersectedCell != null) intersectedCell.CurrentCharacter = null;

                selectedCharacter = character;
                _charactersOnField.Remove(character);
                break;
            }
            
            character.UpdatePosition();
        }

        foreach (var character in _charactersOnStore)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && character.IsClicked(Mouse.GetState()) &&
                selectedCharacter == null)
            {
                selectedCharacter = character.GetCopy(GraphicsDevice);
                break;
            }
        }

        if (Mouse.GetState().LeftButton == ButtonState.Released && selectedCharacter != null)
        {
            var intersectedCell = GetIntersectedCell(Mouse.GetState().Position.ToVector2());
            if (intersectedCell != null && intersectedCell.CurrentCharacter == null)
            {
                _charactersOnField.Add(selectedCharacter);
                selectedCharacter.SetNewPosition(intersectedCell.GetCharacterPosition(selectedCharacter));
                intersectedCell.CurrentCharacter = selectedCharacter;
                selectedCharacter.FieldCoordinates = intersectedCell.FieldCoordinates;
                selectedCharacter = null;
            }
        }

        foreach (var fieldCell in _field)
        {
            if (fieldCell.CurrentCharacter != null && fieldCell.CurrentCharacter.Health < 1) fieldCell.CurrentCharacter = null;
        }
        _charactersOnField = _charactersOnField.Where(x => x.Health > 0).ToList();
        _previousButtonState = Mouse.GetState().LeftButton;
        base.Update(gameTime);
        _framesCounter++;
        if (_framesCounter == _movesLength + 1)
        {
            _framesCounter = 1;
        }
    }

    private void DrawLineBetween(
        Vector2 startPos,
        Vector2 endPos)
    {
        var dx = startPos.X - endPos.X;
        var dy = startPos.Y - endPos.Y;
        var tg = dx / dy;
        var angle = (float)-Math.Atan(tg) - (dy < 0 ? Math.PI : 0);   
        var distance = Vector2.Distance(startPos, endPos);
        var scale = distance / _line.Height;


        _spriteBatch.Draw(_line, startPos, null, 
            Color.Black, (float)(angle + Math.PI),Vector2.Zero, scale, SpriteEffects.None, 1);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gray);
        _spriteBatch.Begin();
        _spriteBatch.Draw(_fon, new Vector2(-200, -250), null, Color.White, 0,
            Vector2.Zero, 0.45f, SpriteEffects.None, 0);
        
        // Y lines
        DrawLineBetween(new Vector2(4, 0), new Vector2(4, 720));
        DrawLineBetween(new Vector2(1077, 0), new Vector2(1077, 720));
        DrawLineBetween(new Vector2(950, 0), new Vector2(950, 720));
        //      game place
        DrawLineBetween(new Vector2(22, 576), new Vector2(50, 100));
        DrawLineBetween(new Vector2(900, 100), new Vector2(928, 576));

        var upperX = 50f;
        
         for (var x = 22 + _cellSize.Width; x < 920; x += _cellSize.Width)
         { 
             upperX += _cellSize.Width * 0.939f;
             DrawLineBetween(new Vector2(x, 576), new Vector2(upperX, 100));
         }
         
        // X lines
        DrawLineBetween(new Vector2(0, 3), new Vector2(1078, 3));
        DrawLineBetween(new Vector2(0, 716), new Vector2(1078, 716));
        DrawLineBetween(new Vector2(0, 586), new Vector2(950, 586));
        //      game place

        var gamePlaceHeight = 476;
        var j = 0;
        for (var i = 0f; i <= gamePlaceHeight; i += _cellSize.Height)
        {
            DrawLineBetween(new Vector2(50 - j, 100 + i), new Vector2(900 + j, 100 + i));
            j += 4;
        }
        

        if (selectedCharacter != null)
        {
            _spriteBatch.Draw(selectedCharacter.GetCurrentImage(),
                selectedCharacter.GetPositionForCenterDrawingOnField(Mouse.GetState().Position.ToVector2()),
                null,
                Color.White, rotation: 0, origin: Vector2.Zero,
                selectedCharacter.ImageScaleOnField, Mouse.GetState().Position.X > 470 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        foreach (var character in _charactersOnStore)
        {
            _spriteBatch.Draw(character.GetCurrentImage(),
                character.ImageLocation,
                null,
                Color.White, 0, origin: Vector2.Zero,
                character.ImageScaleOnStore, SpriteEffects.None, 0);
            //_spriteBatch.DrawString(_font, character.Price.ToString(), character.ImageLocation + new Vector2(0, 50),
            //    Color.Black);
        }

        foreach (var character in _charactersOnField)
        {
            _spriteBatch.Draw(character.GetCurrentImage(),
                character.ImageLocation,
                null,
                Color.White, 0, Vector2.Zero,
                character.ImageScaleOnField, character.Player == Player.Computer ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
        //_spriteBatch.Draw(mySpriteTexture, new Vector2(X,Y), Color.White);
        foreach (var cell in _field)
        {
            if (cell.CurrentCharacter != null)
            {
                _spriteBatch.Draw(cell.CurrentCharacter.Player == Player.Human ? _greenRect : _redRect, cell.Coordinates, null, Color.White, 0, 
                    Vector2.Zero, 0.2f, SpriteEffects.None, 0);
                _spriteBatch.DrawString(_font, cell.CurrentCharacter.Health.ToString(), cell.Coordinates, Color.Orange);
            }
        }
        _spawnButton.Draw(_spriteBatch);
        
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }

    private FieldCell GetIntersectedCell(Vector2 location)
    {
        FieldCell intersectedCell = null;

        for (var i = 0; i < _field.GetLength(0); i++)
        {
            for (var j = 0; j < _field.GetLength(1); j++)
            {
                if (_field[i, j].IsIntersected(location))
                {
                    intersectedCell = _field[i, j];
                    break;
                }
            }

            if (intersectedCell != null)
            {
                break;
            }

        }
        return intersectedCell;
        
    }
}