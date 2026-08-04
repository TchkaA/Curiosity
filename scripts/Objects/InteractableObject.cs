using Godot;
using System;

public partial class InteractableObject : Node2D, IInteractable
{
	public Sprite2D Visual;
	public Shader shader => GD.Load<Shader>("res://shaders/outline/outline.gdshader");
	public ShaderMaterial material;

	public override void _Ready()
	{
		InitShader();
	}

	public virtual void Interact(Node2D interactor)
	{
		GD.Print("sosi hyi");
	}

    public virtual void InteractEnter()
    {
        if (material == null)
            return;

        material.SetShaderParameter("outline_size", 0.8f);
    }

    public virtual void InteractExit()
	{
		if (material == null)
			return;

		material.SetShaderParameter("outline_size", 0f);
	}


	private void InitShader()
	{
		Visual = GetNode<Sprite2D>("Sprite2D");

		material = new ShaderMaterial();
        material.Shader = shader;
		material.SetShaderParameter("outline_size", 0f);
        Visual.Material = material;
	}
}
