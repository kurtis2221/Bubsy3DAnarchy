using Godot;
using System;
using System.Collections.Generic;

public partial class MenuControl : Control
{
    public static MenuControl inst;

    public static bool pause;

    const string file_cfg = "config.ini";

    static ini_hndl.IniHandler cfg = new ini_hndl.IniHandler(file_cfg);

    [Export]
    public CheckBox shadow_enable;

    [Export]
    public CheckBox arrow_enable;

    [Export]
    public CheckBox music_enable;

    [Export]
    public CheckBox nightmare_mode;

    [Export]
    public HSlider mouse_sens;

    [Export]
    public Label mouse_sens_val;

    [Export]
    public HSlider volume;

    [Export]
    public Control keybinds;

    [Export]
    public Button save_opt;

    [Export]
    public Button quit_game;

    [Export]
    public Light3D map_light;

    [Export]
    public WorldEnvironment env;

    [Export]
    public Node3D dynamics;

    [Export]
    public Control invert_screen;

    List<BindButton> keybind_list = new List<BindButton>();

    int bus_master = AudioServer.GetBusIndex("Master");

    const float pitch_normal = 1.0f;
    const float pitch_nightmare = 0.3f;

    public override void _Ready()
    {
        inst = this;
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        //Keys
        LoadKeybindNames();
        //Events
        shadow_enable.Toggled += Shadow_enable_Toggled;
        arrow_enable.Toggled += Arrow_enable_Toggled;
        music_enable.Toggled += Music_enable_Toggled;
        nightmare_mode.Toggled += Nightmare_mode_Toggled;
        volume.ValueChanged += Volume_ValueChanged;
        mouse_sens.ValueChanged += Mouse_sens_ValueChanged;
        save_opt.Pressed += Save_opt_Pressed;
        quit_game.Pressed += Quit_game_Pressed;
        mouse_sens.Value = 2.0f;
        //Config
        LoadData();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!pause) return;
        BindButton btn = BindButton.assign;
        if (btn == null) return;
        InputEventKey key = @event as InputEventKey;
        if (key == null) return;
        if (key.PhysicalKeycode != Key.Escape)
        {
            btn.Text = key.AsTextPhysicalKeycode();
            btn.keyevent.PhysicalKeycode = key.PhysicalKeycode;
        }
        else
        {
            btn.Text = btn.keyevent.AsTextPhysicalKeycode();
        }
        BindButton.assign = null;
    }

    private void LoadKeybindNames()
    {
        bool read = false;
        int i = 0;
        foreach (string action in InputMap.GetActions())
        {
            if (!read && action == "Forward") read = true;

            if (read)
            {
                Label lbl = new Label();
                lbl.Name = "KeyBindLabel" + i;
                lbl.Text = action;
                lbl.Position = new Vector2(0, 64 + 32 * i);
                lbl.Size = new Vector2(128, 32);
                keybinds.AddChild(lbl);
                BindButton btn = new BindButton();
                btn.Position = new Vector2(128, 64 + 32 * i);
                btn.Size = new Vector2(128, 32);
                InputEventKey keyevent = (InputEventKey)InputMap.ActionGetEvents(action)[0];
                btn.Name = "KeyBindButton" + i;
                btn.Text = keyevent.AsTextPhysicalKeycode();
                btn.keyevent = keyevent;
                btn.label = lbl;
                keybinds.AddChild(btn);
                keybind_list.Add(btn);
                i++;
            }
        }
    }

    //Config load
    private void LoadData()
    {
        try
        {
            cfg.LoadIni();
            cfg.SelectSection("");
            mouse_sens.Value = float.Parse(cfg["MouseSens"]);
            shadow_enable.ButtonPressed = cfg["EnableShadow"] != "0";
            arrow_enable.ButtonPressed = cfg["Arrow"] != "0";
            music_enable.ButtonPressed = cfg["Music"] != "0";
            volume.Value = float.Parse(cfg["Volume"]);
            nightmare_mode.ButtonPressed = cfg["Nightmare"] != "0";
            cfg.SelectSection("Keys");
            foreach (BindButton btn in keybind_list)
            {
                string action = btn.label.Text;
                string keycode = cfg[action];
                if (keycode == null) continue;
                try
                {
                    Key key = Enum.Parse<Key>(keycode);
                    btn.keyevent.PhysicalKeycode = key;
                    btn.Text = btn.keyevent.AsTextPhysicalKeycode();
                }
                catch { }
                if (action == "Restart")
                {
                    PlayerControl.inst.msg_restart.Text = "Presss " + btn.keyevent.AsTextPhysicalKeycode() + " to restart";
                }
            }
        }
        catch { }
    }

    //Config save
    private void SaveData()
    {
        try
        {
            cfg.SelectSection("");
            cfg["MouseSens"] = mouse_sens.Value.ToString("0.0");
            cfg["EnableShadow"] = shadow_enable.ButtonPressed ? "1" : "0";
            cfg["Arrow"] = arrow_enable.ButtonPressed ? "1" : "0";
            cfg["Music"] = music_enable.ButtonPressed ? "1" : "0";
            cfg["Volume"] = volume.Value.ToString("0.00");
            cfg["Nightmare"] = nightmare_mode.ButtonPressed ? "1" : "0";
            cfg.SelectSection("Keys");
            foreach (BindButton btn in keybind_list)
            {
                string action = btn.label.Text;
                cfg[action] = btn.keyevent.AsTextPhysicalKeycode();
            }
            cfg.SaveIni();
        }
        catch { }
    }

    private void Mouse_sens_ValueChanged(double value)
    {
        mouse_sens_val.Text = value.ToString("0.0");
        CameraControl.mouse_sens = (float)(value / 10);
    }

    private void Volume_ValueChanged(double value)
    {
        AudioServer.SetBusVolumeDb(bus_master, (float)Mathf.LinearToDb(value));
    }

    private void Shadow_enable_Toggled(bool toggledOn)
    {
        map_light.ShadowEnabled = toggledOn;
        map_light.LightEnergy = toggledOn ? 1.0f : 1.5f;
    }

    private void Arrow_enable_Toggled(bool toggledOn)
    {
        PlayerControl.inst.arrow_gui.Visible = toggledOn;
    }

    private void Music_enable_Toggled(bool toggledOn)
    {
        if (toggledOn)
        {
            PlayerControl.inst.music.Play();
        }
        else
        {
            PlayerControl.inst.music.Stop();
        }
    }

    private void Nightmare_mode_Toggled(bool toggledOn)
    {
        if (toggledOn)
        {
            invert_screen.Visible = true;
            PlayerControl.inst.ChangeSounds(pitch_nightmare);
            ChangeAllNodes<AudioStreamPlayer3D>(dynamics, x => x.PitchScale = pitch_nightmare);
        }
        else
        {
            invert_screen.Visible = false;
            PlayerControl.inst.ChangeSounds(pitch_normal);
            ChangeAllNodes<AudioStreamPlayer3D>(dynamics, x => x.PitchScale = pitch_normal);
        }
    }

    private void ChangeAllNodes<T>(Node node, Action<T> hndl) where T : Node
    {
        foreach (Node ch in node.GetChildren())
        {
            if (ch is T)
            {
                hndl((T)ch);
            }
            ChangeAllNodes(ch, hndl);
        }
    }

    private void Save_opt_Pressed()
    {
        SaveData();
    }

    private void Quit_game_Pressed()
    {
        GetTree().Quit();
    }
}
