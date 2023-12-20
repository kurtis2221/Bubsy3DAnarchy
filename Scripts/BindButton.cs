using Godot;
using System;

public partial class BindButton : Button
{
    public static BindButton assign;
    public InputEventKey keyevent;
    public Label label;

    public override void _Pressed()
    {
        if (assign != null) return;
        Text = "Press a key...";
        assign = this;
    }
}
