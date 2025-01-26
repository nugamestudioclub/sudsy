using Godot;
using System;

public class Dirt : Area2D {

    [Export]
    private float _startingAmount = 10f;
    private float _amount;
    [Export]
    public float Amount
    {
        get => _amount; 
        set
        {
            _amount = value;
            //_sprite.Modulate = new Color(
            //    _sprite.Modulate.r, 
            //    _sprite.Modulate.g,
            //    _sprite.Modulate.b,
            //    _amount / _startingAmount);
            //_sprite.Material = (Material)_sprite.Material.Duplicate();
            ((ShaderMaterial)_sprite.Material).SetShaderParam("opacity", _amount / _startingAmount);
        }
    }

    private Sprite _sprite;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("Sprite");
        Amount = _startingAmount;
        ((ShaderMaterial)_sprite.Material).SetShaderParam("global_transform", GlobalTransform);
    }


    
}
