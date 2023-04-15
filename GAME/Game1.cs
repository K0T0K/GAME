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
        _characters.Add(new Knight(_graphics.GraphicsDevice));
        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
        foreach (var character in _characters)
        {
            character.State = CharacterState.Idle;
        }

        foreach (var character in _characters)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && character.IsClicked(Mouse.GetState()))
            {
                selectedCharacter = character;
                _characters.Remove(character);
                break;
            }
        }

        if (Mouse.GetState().LeftButton == ButtonState.Released && selectedCharacter != null)
        {
            _characters.Add(selectedCharacter);
            selectedCharacter.Location = Mouse.GetState().Position.ToVector2();
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
            _spriteBatch. Draw(texture: selectedCharacter.GetCurrentImage(), Mouse.GetState().Position. ToVector2(),
                sourceRectangle: null,
                Color. White, rotation: 0, origin: Vector2.Zero,
                scale: 0.3f, SpriteEffects.None, layerDepth: 0);
        }

        foreach (var character in _characters)
        {
            _spriteBatch.Draw(character.GetCurrentImage(), 
                character.Location, 
                null, 
                Color.White, 0, Vector2.Zero, 
                0.3f, SpriteEffects.None, 0);
        }
        
        // Y lines
        DrawLineBetween(new Vector2(4, 0), new Vector2(4, 720), 7, Color.Black);
        DrawLineBetween(new Vector2(1077, 0), new Vector2(1077, 720), 7, Color.Black);
        // X lines
        DrawLineBetween(new Vector2(0, 3), new Vector2(1078, 3), 7, Color.Black);
        DrawLineBetween(new Vector2(0, 716), new Vector2(1078, 716), 7, Color.Black);
        DrawLineBetween(new Vector2(0, 716), new Vector2(1078, 716), 7, Color.Black);
        //_spriteBatch.Draw(mySpriteTexture, new Vector2(X,Y), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}