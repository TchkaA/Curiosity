using Godot;

/// <summary>
/// Панель осмотра предмета: иконка, название и описание.
/// </summary>
[GlobalClass]
public partial class InspectPanel : GamePanel
{
    [Export] public Label NameLabel;
    [Export] public TextureRect IconTexture;
    [Export] public Label DescriptionLabel;

    /// <summary>
    /// Показывает данные предмета.
    /// </summary>
    public void ShowItem(Item item)
    {
        if (item == null)
            return;

        if (NameLabel != null)
            NameLabel.Text = item.Name;

        if (IconTexture != null)
            IconTexture.Texture = item.Icon;

        if (DescriptionLabel != null)
            DescriptionLabel.Text = item.Description;

        Show();
    }
}
