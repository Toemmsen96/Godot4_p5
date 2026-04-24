using Godot;

public partial class Demo : SubViewportContainer
{
	[Export]
	public Script Sketch { get; set; } = null!;

	private SubViewport sketchViewport = null!;
	private ColorRect viewportBg = null!;
	private Node2D canvas = null!;
	private CanvasLayer canvasLayer = null!;
	private Label labelWarningMsg = null!;
	private TextureButton btMenu = null!;
	private Panel panel = null!;
	private Label lbFps = null!;
	private ColorPickerButton btCurrentColor = null!;
	private FileDialog fileDialog = null!;
	private Image? imgSave;

	private string currentSketchPath = string.Empty;

	public override void _Ready()
	{
		sketchViewport = GetNode<SubViewport>("SketchViewport");
		viewportBg = GetNode<ColorRect>("SketchViewport/ViewportBg");
		canvas = GetNode<Node2D>("SketchViewport/Canvas");
		canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
		labelWarningMsg = GetNode<Label>("CanvasLayer/LabelWarningMsg");
		btMenu = GetNode<TextureButton>("CanvasLayer/BtMenu");
		panel = GetNode<Panel>("CanvasLayer/Panel");
		lbFps = GetNode<Label>("CanvasLayer/Panel/BoxButton/LabelFps");
		btCurrentColor = GetNode<ColorPickerButton>("CanvasLayer/Panel/BoxButton/BtCurrentColor");
		fileDialog = GetNode<FileDialog>("FileDialog");

		if (Sketch == null)
		{
			labelWarningMsg.Visible = true;
		}
		else
		{
			currentSketchPath = Sketch.ResourcePath;
			LoadSketch();
			btMenu.Show();
		}

		sketchViewport.HandleInputLocally = true;
	}

	private void LoadSketch()
	{
		canvas.SetScript(Sketch);

		canvas = GetNode<Node2D>("SketchViewport/Canvas");
		if (canvas is not GodotP5 p5Canvas)
		{
			GD.PushError("Sketch script must inherit from GodotP5.");
			return;
		}

		p5Canvas.Connect(GodotP5.SignalName.SetBackgroundColor, new Callable(this, nameof(SetBackgroundColor)));
		p5Canvas.Connect(GodotP5.SignalName.SetViewportSize, new Callable(this, nameof(SetViewportSize)));
		p5Canvas.Connect(GodotP5.SignalName.SetCurrentColor, new Callable(this, nameof(SetCurrentColor)));

		p5Canvas.SubViewport = sketchViewport;
		p5Canvas.InitFromMainScene();
	}

	private void SetBackgroundColor(Color color)
	{
		viewportBg.Color = color;
	}

	private void SetViewportSize(Vector2I viewportSize)
	{
		sketchViewport.Set("size", viewportSize);
		sketchViewport.Set("size_2d_override", viewportSize);
		viewportBg.Set("size", new Vector2(viewportSize.X, viewportSize.Y));
		DisplayServer.WindowSetSize(viewportSize);
	}

	private void SetCurrentColor(Color color)
	{
		btCurrentColor.Color = color;
	}

	private void OnWindowSizeChanged()
	{
		GD.Print($"window size changed : {GetViewportRect().Size}");
	}

	private void _OnBtMenuPressed()
	{
		panel.Show();
		btMenu.Hide();
	}

	private void _OnBtHidePressed()
	{
		panel.Hide();
		btMenu.Show();
	}

	private void _OnBtPausePressed()
	{
		GD.Print("on Button pause pressed ..");
		if (canvas is GodotP5 p5Canvas)
		{
			p5Canvas.Pause();
		}
	}

	private void _OnBtRestartPressed()
	{
		sketchViewport.Set("size", Vector2I.Zero);
		if (canvas is GodotP5 p5Canvas)
		{
			p5Canvas.Restart();
		}
	}

	private void _OnColorBtCurrentColorChanged(Color color)
	{
		if (canvas is GodotP5 p5Canvas)
		{
			p5Canvas.CurrentColor = color;
		}
	}

	private void _OnBtSaveImagePressed()
	{
		imgSave = sketchViewport.GetTexture().GetImage();
		fileDialog.Show();
	}

	private void _OnFileDialogFileSelected(string path)
	{
		imgSave?.SavePng(path);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Sketch == null)
		{
			return;
		}

		canvas.Call("_unhandled_input", @event);
	}
}
