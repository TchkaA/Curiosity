using Godot;

public partial class InteractableObject : Node2D, IInteractable
{
	public Sprite2D Visual;
	public Shader shader => MainManager.Instance.OutlineShader;
	public ShaderMaterial material;

	public override void _Ready()
	{
		InitShader();
	}

	public virtual void Interact(BaseEntity interactor)
	{
		// Поведение задают наследники (например, Chest).
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
