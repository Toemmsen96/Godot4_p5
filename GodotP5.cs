#nullable enable
using Godot;

public partial class GodotP5 : Node2D
{
    [Signal]
    public delegate void SetBackgroundColorEventHandler(Color color);

    [Signal]
    public delegate void SetViewportSizeEventHandler(Vector2I viewportSize);

    [Signal]
    public delegate void SetCurrentColorEventHandler(Color color);

    public enum ViewportMode
    {
        Always,
        Never,
        Once,
    }

    public SubViewport SubViewport { get; set; } = null!;

    public Color CurrentBackgroundColor = Colors.Black;
    public Color CurrentColor = Colors.White;

    public float Width { get; protected set; } = 100;
    public float Height { get; protected set; } = 100;
    public int FrameCount { get; protected set; }
    public float DeltaTime { get; protected set; }
    public int MouseX { get; protected set; }
    public int MouseY { get; protected set; }
    public bool MouseIsPressed { get; protected set; }
    public string? MouseButton { get; protected set; }

    protected Color FillColor = Colors.White;
    protected Color StrokeColor = Colors.Gray;
    protected float StrokeWeight = 1.0f;
    protected bool NoStrokeEnabled;
    protected bool NoFillEnabled;
    protected bool IsLoaded;
    protected bool IsLooping = true;
    protected bool Antialiased = true;

    public const float TWO_PI = Mathf.Tau;
    public const float HALF_PI = Mathf.Pi / 2.0f;
    public const float QUARTER_PI = Mathf.Pi / 4.0f;

    public void InitFromMainScene()
    {
        Setup();
        IsLoaded = true;

        if (!IsLooping)
        {
            QueueRedraw();
        }
        else
        {
            Loop();
        }
    }

    public void SetTitle(string title)
    {
        DisplayServer.WindowSetTitle(title);
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GetTree().Quit();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            MouseIsPressed = mouseButtonEvent.Pressed;
            MouseButton = mouseButtonEvent.ButtonIndex switch
            {
                Godot.MouseButton.Left => "LEFT",
                Godot.MouseButton.Middle => "CENTER",
                Godot.MouseButton.Right => "RIGHT",
                _ => MouseButton,
            };
        }
    }

    public void SetViewportMode(ViewportMode mode)
    {
        if (SubViewport == null)
        {
            return;
        }

        int clearMode = mode switch
        {
            ViewportMode.Always => 0,
            ViewportMode.Never => 1,
            ViewportMode.Once => 2,
            _ => 0,
        };

        SubViewport.Set("render_target_clear_mode", clearMode);
    }

    public override void _Process(double delta)
    {
        if (!IsLoaded)
        {
            return;
        }

        if (IsLooping)
        {
            QueueRedraw();
            FrameCount += 1;
            DeltaTime = (float)delta;
        }

        if (MouseIsPressed)
        {
            MousePressed();
        }

        Vector2 mousePosition = GetLocalMousePosition();
        MouseX = (int)mousePosition.X;
        MouseY = (int)mousePosition.Y;
    }

    public void Pause()
    {
        SetProcess(!IsProcessing());
    }

    public void Restart()
    {
        Setup();
    }

    public void CreateCanvas(int width, int height)
    {
        Width = width;
        Height = height;
        EmitSignal(SignalName.SetViewportSize, new Vector2I(width, height));
    }

    public void Background(Color color, float alpha = -1)
    {
        CurrentBackgroundColor = color;
        EmitSignal(SignalName.SetBackgroundColor, color);
    }

    public void NoStroke()
    {
        NoStrokeEnabled = true;
    }

    public void NoFill()
    {
        NoFillEnabled = true;
    }

    public void Fill(Color color)
    {
        FillColor = color;
        NoFillEnabled = false;
    }

    public void Stroke(Color color)
    {
        StrokeColor = color;
        NoStrokeEnabled = false;
    }

    public void SetColor(Color color)
    {
        CurrentColor = color;
        FillColor = color;
        StrokeColor = color;
        NoFillEnabled = false;
        NoStrokeEnabled = false;
    }

    public void StrokeWeightSet(float weight)
    {
        StrokeWeight = weight;
    }

    public void Circle(float x, float y, float radius, int pointCount = 32)
    {
        if (!NoFillEnabled)
        {
            DrawCircle(new Vector2(x, y), radius, FillColor);
        }

        if (!NoStrokeEnabled)
        {
            DrawArc(new Vector2(x, y), radius, 0, Mathf.Tau, pointCount, StrokeColor, StrokeWeight, Antialiased);
        }
    }

    public void Line(float x0, float y0, float x1, float y1)
    {
        if (NoStrokeEnabled)
        {
            return;
        }

        DrawLine(new Vector2(x0, y0), new Vector2(x1, y1), StrokeColor, StrokeWeight, Antialiased);
    }

    public void DrawSketchString(Font font, Vector2 position, string text, HorizontalAlignment alignment, int width, int fontSize, Color modulate)
    {
        DrawString(font, position, text, alignment, width, fontSize, modulate);
    }

    public void NoLoop()
    {
        IsLooping = false;
        SetProcess(false);
    }

    public void Loop()
    {
        IsLooping = true;
        SetProcess(true);
    }

    public virtual void Setup()
    {
    }

    public virtual void DrawSketch()
    {
    }

    public override void _Draw()
    {
        DrawSketch();
    }

    public void MousePressed()
    {
    }
}
