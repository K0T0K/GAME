using System;
using System.Collections.Generic;
using System.Drawing;
using Game1.Characters;
using Game1.Characters.Enums;
using Game1.Characters.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;



namespace GAME;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private List<ICharacter> _charactersOnField = new();
    private List<ICharacter> _charactersOnStore = new();
    private ICharacter selectedCharacter;
    private Size _cellSize = new(40, 68);
    private Vector2 _storeLocation = new(-50, 555);
    private SpriteFont _font;
    private Texture2D _line;

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

        _line = Texture2D.FromFile(GraphicsDevice, @"Images\Pictures\line.png");

        _charactersOnStore.Add(new Knight(GraphicsDevice, 100, _storeLocation, 0.2f));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
        foreach (var character in _charactersOnField)
        {
            character.State = CharacterState.Walk;
        }

        foreach (var character in _charactersOnField)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && character.IsClicked(Mouse.GetState()) &&
                selectedCharacter == null)
            {
                selectedCharacter = character;
                _charactersOnField.Remove(character);
                break;
            }
            
        }

        if (Mouse.GetState().LeftButton == ButtonState.Released && selectedCharacter != null)
        {
            _charactersOnField.Add(selectedCharacter);
            selectedCharacter.ImageLocation =
                selectedCharacter.GetPositionForCenterDrawing(Mouse.GetState().Position.ToVector2());
            selectedCharacter = null;
        }

        base.Update(gameTime);

    }

    private void DrawLineBetween(
        Vector2 startPos,
        Vector2 endPos)
    {
        var dx = startPos.X - endPos.X;
        var dy = startPos.Y - endPos.Y;
        var tg = dx / dy;
        var angle = (float)-Math.Atan(tg) - (dy < 0 ? Math.PI : 0);    /////////
        var distance = Vector2.Distance(startPos, endPos);
        var scale = distance / _line.Height;


        _spriteBatch.Draw(_line, startPos, null, 
            Color.Black, (float)(angle + Math.PI),Vector2.Zero, scale, SpriteEffects.None, 1);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gray);
        _spriteBatch.Begin();


        // Y lines
        DrawLineBetween(new Vector2(4, 0), new Vector2(4, 720));
        DrawLineBetween(new Vector2(1077, 0), new Vector2(1077, 720));
        DrawLineBetween(new Vector2(950, 0), new Vector2(950, 720));
        //      game place
        DrawLineBetween(new Vector2(22, 576), new Vector2(50, 100));
        DrawLineBetween(new Vector2(900, 100), new Vector2(928, 576));

        
         for (var a = 0; a <= 900; a++)
        {
            DrawLineBetween(new Vector2(22, 576), new Vector2(50, 100));
        }



        // X lines
        DrawLineBetween(new Vector2(0, 3), new Vector2(1078, 3));
        DrawLineBetween(new Vector2(0, 716), new Vector2(1078, 716));
        DrawLineBetween(new Vector2(0, 586), new Vector2(950, 586));
        //      game place

        var gamePlaceHeight = 476;
        var j = 0;
        for (var i = 0; i <= gamePlaceHeight; i += _cellSize.Height)
        {
            DrawLineBetween(new Vector2(50 - j, 100 + i), new Vector2(900 + j, 100 + i));
            j += 4;
        }
        

        if (selectedCharacter != null)

        {
            _spriteBatch.Draw(texture: selectedCharacter.GetCurrentImage(),
                selectedCharacter.GetPositionForCenterDrawing(Mouse.GetState().Position.ToVector2()),
                sourceRectangle: null,
                Color.White, rotation: 8, origin: Vector2.Zero,
                scale: selectedCharacter.ImageScale, SpriteEffects.None, layerDepth: 0);
        }

        foreach (var character in _charactersOnStore)
        {
            _spriteBatch.Draw(character.GetCurrentImage(),
                character.ImageLocation,
                null,
                Color.White, 0, origin: Vector2.Zero,
                character.ImageScale, SpriteEffects.None, 0);
            //_spriteBatch.DrawString(_font, character.Price.ToString(), character.ImageLocation + new Vector2(0, 50),
            //    Color.Black);
        }

        foreach (var character in _charactersOnField)
        {

            _spriteBatch.Draw(character.GetCurrentImage(),
                character.ImageLocation,
                null,
                Color.White, 0, Vector2.Zero,
                character.ImageScale, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_font, character.Price.ToString(), character.ImageLocation + new Vector2(100, 120),
                Color.Black);

        }
        //_spriteBatch.Draw(mySpriteTexture, new Vector2(X,Y), Color.White);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}