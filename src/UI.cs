using Godot;

public class UI : Node2D {
	private Sprite _soapBar;

	private RichTextLabel _cleanLabel;

	public override void _Ready() {
		_soapBar = GetNode<Sprite>("HBoxContainer/Soap/Fill");
		_cleanLabel = GetNode<RichTextLabel>("HBoxContainer/Clean/RichTextLabel");
	}

	public void DrawClean(float value) {
		_cleanLabel.Text = $"{(int)Mathf.Ceil(value * 100)}%";
	}

	public void DrawSoap(float value) {
		_soapBar.Scale = new Vector2(value, 1f);
	}
}