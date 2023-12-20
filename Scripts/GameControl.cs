using Godot;
using System;

public partial class GameControl : Node3D
{
    [Export]
    public Control menu;

    bool is_quit;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Window win = GetWindow();
        win.GrabFocus();
#if DEBUG
        win.Mode = Window.ModeEnum.Windowed;
        win.Size = new Vector2I(1280, 720);
#endif
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel") && BindButton.assign == null)
        {
            menu.Visible = !menu.Visible;
            MenuControl.pause = menu.Visible;
            Input.MouseMode = menu.Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
        }
    }
}