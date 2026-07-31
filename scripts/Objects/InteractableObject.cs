using Godot;
using System;

public partial class InteractableObject : Node2D, IInteractable
{
	private Sprite2D Visual => GetNode<Sprite2D>("Sprite2D");
	protected Shader shader = GD.Load<Shader>("res://shaders/outline/outline.gdshader");
	protected ShaderMaterial material;
	public override void _Ready()
	{
		InitShader();
	}

	public virtual void Interact()
	{
		GD.Print("sosi hyi");
	}

    public virtual void InteractEnter()
    {
        material.SetShaderParameter("outline_size", 0.8f);
    }

    public virtual void InteractExit()
	{
		material.SetShaderParameter("outline_size", 0f);
	}


	private void InitShader()
	{
		material = new ShaderMaterial();
        material.Shader = shader;
		material.SetShaderParameter("outline_size", 0f);
        Visual.Material = material;
	}
}
