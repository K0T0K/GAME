using Game1.Characters.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1.Characters.Interfaces;

public interface ICharacter
{
    public Vector2 Location { get; set; }
    public Texture2D GetCurrentImage();
    public void Move(Vector2 delta);
    public CharacterState State { get; set; }
    public bool IsClicked(MouseState mousePoint);
}