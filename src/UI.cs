using Godot;
using System;

public class UI : Node2D {
	private Sprite _soapBar;

	private RichTextLabel _cleanLabel;

	private RichTextLabel _timeLabel;

	public override void _Ready() {
		_soapBar = GetNode<Sprite>("HBoxContainer/Soap/Fill");
		_cleanLabel = GetNode<RichTextLabel>("HBoxContainer/Clean/RichTextLabel");
		_timeLabel = GetNode<RichTextLabel>("HBoxContainer/Time/RichTextLabel");
	}

	public void DrawClean(float value) {
		_cleanLabel.Text = $"{(int)Mathf.Ceil(value * 100)}%";
	}

	public void DrawSoap(float value) {
		_soapBar.Scale = new Vector2(value, 1f);
	}

	public void DrawTime(float value) {
		var dt = new DateTime().AddSeconds(value);
		_timeLabel.BbcodeText = $"[right]{dt:mm:ss}[/right]";
		_timeLabel.Modulate = value < 60
			? new Color(1f, 0f, 0f)
			: new Color(1f, 1f, 1f);
	}
}