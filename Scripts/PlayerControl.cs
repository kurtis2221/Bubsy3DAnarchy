using Godot;
using System;

public partial class PlayerControl : CharacterBody3D
{
    public static PlayerControl inst;

    enum PlayerAnimType
    {
        Idle,
        Walk,
        Run,
        Jump,
        Death
    }

    public enum PlayerSndType
    {
        Jump,
        Land,
        Death,
        Block,
        Check,
        End
    }

    [Export]
    public ShapeCast3D plat_scan;

    [Export]
    public Node3D char_tr;

    [Export]
    public Node3D char_obj;

    [Export]
    public Node3D cam_y;

    [Export]
    public AudioStream[] sounds;

    [Export]
    public Node3D root;

    [Export]
    public Label msg_restart;

    [Export]
    public Control msg_win;

    [Export]
    public Control arrow_gui;

    [Export]
    public AudioStreamPlayer music;

    AudioStreamPlayer[] snd_players;

    AnimationPlayer anim;
    PlayerAnimType curr_anim;

    public Vector3 last_cp;

    const float walk_speed = 5.0f;
    const float run_speed = 10.0f;
    const float jump_speed = 8.0f;

    float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    bool old_on_floor;
    public bool block;
    bool block_keys;

    public override void _Ready()
    {
        inst = this;
        last_cp = GlobalPosition;
        foreach (Node node in char_obj.GetChildren())
        {
            if (node is AnimationPlayer)
            {
                anim = (AnimationPlayer)node;
                break;
            }
        }
        snd_players = new AudioStreamPlayer[sounds.Length];
        for (int i = 0; i < sounds.Length; i++)
        {
            AudioStreamPlayer tmp = new AudioStreamPlayer();
            tmp.Stream = sounds[i];
            snd_players[i] = tmp;
            root.AddChild(tmp);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (MenuControl.pause) return;

        Vector3 velocity = Velocity;
        bool is_running = Input.IsActionPressed("Run");
        bool on_floor = IsOnFloor();
        float speed = is_running ? run_speed : walk_speed;

        if (plat_scan.GetCollisionCount() > 0)
        {
            GodotObject obj = plat_scan.GetCollider(0);
            if (obj != null)
            {
                CollisionObject3D tmp = obj as CollisionObject3D;
                if (tmp == null || tmp.CollisionLayer == 1)
                {
                    Reparent((Node)obj);
                }
            }
        }
        else
        {
            Reparent(root);
        }

        if (on_floor && !old_on_floor)
        {
            PlaySound(PlayerSndType.Land);
        }

        old_on_floor = on_floor;

        if (!on_floor)
            velocity.Y -= gravity * (float)delta;

        if (Input.IsActionJustPressed("Restart"))
        {
            Respawn();
        }

        if (block)
        {
            velocity.X = 0;
            velocity.Z = 0;
            Velocity = velocity;
            MoveAndSlide();
            return;
        }

        if (Input.IsActionPressed("Jump") && on_floor)
        {
            PlaySound(PlayerSndType.Jump);
            velocity.Y = jump_speed;
        }

        if (Input.IsKeyPressed(Key.Backspace) && Input.IsKeyPressed(Key.C))
        {
            if (!block_keys)
            {
                GlobalPosition = CheckpontHandler.curr_cp_pos;
                block_keys = true;
            }
        }
        else block_keys = false;

        Vector2 inputDir = Input.GetVector("Left", "Right", "Forward", "Backward");
        Vector3 direction = (cam_y.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            Vector3 tmp = char_tr.GlobalRotation;
            tmp.Y = Mathf.LerpAngle(char_tr.GlobalRotation.Y, Mathf.Atan2(direction.X, direction.Z), 0.1f);
            char_tr.GlobalRotation = tmp;
        }

        if (on_floor)
        {
            if (direction != Vector3.Zero)
            {
                if (is_running) PlayAnimation(PlayerAnimType.Run);
                else PlayAnimation(PlayerAnimType.Walk);
            }
            else PlayAnimation(PlayerAnimType.Idle);
        }
        else
        {
            PlayAnimation(PlayerAnimType.Jump);
        }

        velocity.X = direction.X * speed;
        velocity.Z = direction.Z * speed;

        Velocity = velocity;
        ProcessCollisions();
        MoveAndSlide();
    }

    void ProcessCollisions()
    {
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision3D col = GetSlideCollision(i);
            IntObj mov = col.GetCollider() as IntObj;
            if (mov != null) mov.Activate();
        }
    }

    public void PlaySound(PlayerSndType input)
    {
        snd_players[(int)input].Play();
    }

    public void ChangeSounds(float pitch)
    {
        music.PitchScale = pitch;
        foreach (AudioStreamPlayer snd in snd_players)
        {
            snd.PitchScale = pitch;
        }
    }

    void PlayAnimation(PlayerAnimType input)
    {
        if (input == curr_anim) return;
        curr_anim = input;
        anim.Play(input.ToString(), 0.2);
    }

    public void EndGame()
    {
        SetPhysicsProcess(false);
        PlayAnimation(PlayerAnimType.Idle);
        PlaySound(PlayerSndType.End);
        msg_win.Visible = true;
    }

    public void Damage()
    {
        if (block) return;
        block = true;
        msg_restart.Visible = true;
        PlaySound(PlayerSndType.Death);
        PlayAnimation(PlayerAnimType.Death);
    }

    private async void Respawn()
    {
        GlobalPosition = last_cp;
        msg_restart.Visible = false;
        await ToSignal(GetTree().CreateTimer(0.05), "timeout");
        block = false;
    }
}
