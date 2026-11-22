using Godot;

[Tool]
public partial class ObjectRes : Resource
{
    [ExportGroup("Стороны")]
    
    [Export] 
    public Texture2D SpriteDown;
    [Export] 
    public Texture2D SpriteUp;
    [Export] 
    public Texture2D SpriteLeft;
    [Export] 
    public Texture2D SpriteRight;

}