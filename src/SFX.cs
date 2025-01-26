using Godot;
using System.Collections.Generic;

public class SFX : Node {
	private Dictionary<string, AudioStreamPlayer> _audioStreamPlayers = new Dictionary<string, AudioStreamPlayer>();

	public override void _Ready() {
		AddAudioStreamPlayer("Jump");
		AddAudioStreamPlayer("MidairJump");
		AddAudioStreamPlayer("SoaplessMidairJump");
		AddAudioStreamPlayer("Slide");
		AddAudioStreamPlayer("SoaplessSlide");
		AddAudioStreamPlayer("CollectSoap");
	}

	public void Play(string name) {
		var audioStreamPlayer = _audioStreamPlayers[name];
		audioStreamPlayer.Play(0f);
	}

	private void AddAudioStreamPlayer(string path) {
		_audioStreamPlayers[path] = GetNode<AudioStreamPlayer>(path);
	}
}