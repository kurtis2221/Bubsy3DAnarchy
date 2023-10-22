using Godot;
using System;

public partial class GameControl : Node3D
{
    [Export]
    public Control msg_quit;

    bool is_quit;

    public override void _Ready()
    {
        Window win = GetWindow();
        win.GrabFocus();
#if DEBUG
        win.Mode = Window.ModeEnum.Windowed;
        win.Size = new Vector2I(1280, 720);
#endif
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsKeyPressed(Key.Escape))
        {
            is_quit = true;
            msg_quit.Visible = true;
        }
        if (is_quit)
        {
            if (Input.IsKeyPressed(Key.Y))
            {
                GetTree().Quit();
            }
            else if (Input.IsKeyPressed(Key.N))
            {
                msg_quit.Visible = false;
            }
        }
    }
}