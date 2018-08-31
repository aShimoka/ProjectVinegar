using Godot;
using System;

public class Position2D : Godot.Sprite
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        
    }

    public override void _Process(float delta)
    {
        
        this.SetPosition(GetGlobalMousePosition());
    }
}
