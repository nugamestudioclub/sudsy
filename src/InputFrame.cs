using System;
using System.Collections;

public class InputFrame {
	private readonly BitArray _bits = new BitArray(8);

	public bool Get(ButtonKind kind) {
		return _bits[(int)kind];
	}

	public void Set(ButtonKind kind, bool value = true) {
		_bits[(int)kind] = value;
	}

	public void Reset(ButtonKind kind) {
		Set(kind, false);
	}

	public float X =>
		Convert.ToInt32(Get(ButtonKind.Right))
		- Convert.ToInt32(Get(ButtonKind.Left));

	public float Y =>
		Convert.ToInt32(Get(ButtonKind.Up))
		- Convert.ToInt32(Get(ButtonKind.Down));
}