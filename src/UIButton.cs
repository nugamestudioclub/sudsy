using Godot;
using System;

public class UIButton : Button
{
    [Export]
    private string _scene;
    public override void _Pressed()
    {
        GameManager.Instance.ChangeScene(_scene);
    }
}
