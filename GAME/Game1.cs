using System;
using System.Collections.Generic;
using Game1.Characters;
using Game1.Characters.Enums;
using Game1.Characters.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace GAME;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private List<ICharacter> _characters = new();
    private ICharacter selectedCharacter;

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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _characters.Add(new Knight(_graphics.GraphicsDevice, scale: 0.3f));
        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
        foreach (var character in _characters)
        {
            character.State = CharacterState.Walk;
        }

        foreach (var character in _characters)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && character.IsClicked(Mouse.GetState()))
            {
                selectedCharacter = character;
                _characters.Remove(character);
                break;
            }
            // Изменение картинки во время игры

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                character.ImageScale += 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                character.ImageScale -= 0.1f;
            }
        }

        if (Mouse.GetState().LeftButton == ButtonState.Released && selectedCharacter != null)
        {
            _characters.Add(selectedCharacter);
            selectedCharacter.ImageLocation = selectedCharacter.GetPositionForCenterDrawing(Mouse.GetState().Position.ToVector2());
            selectedCharacter = null;
        }
        
        base.Update(gameTime);
            
    }

    public void DrawLineBetween(
        Vector2 startPos,
        Vector2 endPos,
        int thickness,
        Color color)
    {
        // Create a texture as wide as the distance between two points and as high as
        // the desired thickness of the line.
        var distance = (int)Vector2.Distance(startPos, endPos);
        var texture = new Texture2D(_spriteBatch.GraphicsDevice, distance, thickness);

        // Fill texture with given color.
        var data = new Color[distance * thickness];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = color;
        }
        texture.SetData(data);

        // Rotate about the beginning middle of the line.
        var rotation = (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
        var origin = new Vector2(0, thickness / 2);

        _spriteBatch.Draw(
            texture,
            startPos,
            null,
            Color.White,
            rotation,
            origin,
            1.0f,
            SpriteEffects.None,
            1.0f);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gray);
        _spriteBatch.Begin();
        
        if (selectedCharacter != null)
        {
            _spriteBatch. Draw(texture: selectedCharacter.GetCurrentImage(),
                selectedCharacter.GetPositionForCenterDrawing(Mouse.GetState().Position.ToVector2()),
                sourceRectangle: null,
                Color. White, rotation: 0, origin: Vector2.Zero,
                scale: selectedCharacter.ImageScale, SpriteEffects.None, layerDepth: 0);
        }

        foreach (var character in _characters)
        {
            _spriteBatch.Draw(character.GetCurrentImage(), 
                character.ImageLocation, 
                null, 
                Color.White, 0, Vector2.Zero, 
                character.ImageScale, SpriteEffects.None, 0);
        }
        
        // Y lines
        DrawLineBetween(new Vector2(4, 0), new Vector2(4, 720), 7, Color.Black);
        DrawLineBetween(new Vector2(1077, 0), new Vector2(1077, 720), 7, Color.Black);
        DrawLineBetween(new Vector2(950, 0), new Vector2(950, 720), 7, Color.Black);
        //      game place
        DrawLineBetween(new Vector2(22, 576), new Vector2(50, 100), 2, Color.Black);
        DrawLineBetween(new Vector2(900, 100), new Vector2(928, 576), 2, Color.Black);

        for (var a = 0; a <= 900; a++)
        {
            DrawLineBetween(new Vector2(22, 576), new Vector2(50, 100), 2, Color.Black);
        }
       
        
        
        // X lines
        DrawLineBetween(new Vector2(0, 3), new Vector2(1078, 3), 7, Color.Black);
        DrawLineBetween(new Vector2(0, 716), new Vector2(1078, 716), 7, Color.Black);
        DrawLineBetween(new Vector2(0, 586), new Vector2(950, 586), 7, Color.Black);
        //      game place
        
        var gamePlaceHeight = 476;
        var cellHeight = 68;
        var j = 0;
        for (var i = 0; i <= gamePlaceHeight; i += cellHeight)
        {
            DrawLineBetween(new Vector2(50 - j, 100 + i), new Vector2(900 + j, 100 + i), 2, Color.Black);
            j += 4;
        }
        
        //_spriteBatch.Draw(mySpriteTexture, new Vector2(X,Y), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}