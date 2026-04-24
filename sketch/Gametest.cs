using Godot;
using System;
using System.Collections.Generic;

public partial class Gametest : GodotP5
{
    private sealed class NoteState
    {
        public float X { get; set; }
        public float Y { get; set; }
        public bool Hit { get; set; }
    }

    private readonly List<NoteState> notes = new();
    private float speed = 2.0f;
    private int score = 0;
    private float noteLaneX = 100.0f;

    public override void Setup()
    {
        notes.Clear();
        score = 0;

        SetTitle("Gametest");
        SetViewportMode(ViewportMode.Always);
        CreateCanvas(600, 400);
        GD.Randomize();
        Background(new Color(30f / 255f, 30f / 255f, 30f / 255f));

        for (int i = 0; i < 20; i++)
        {
            notes.Add(new NoteState
            {
                X = Width + i * 120,
                Y = (float)GD.RandRange(100.0, 300.0),
                Hit = false,
            });
        }
    }

    public override void DrawSketch()
    {
        Background(new Color(30f / 255f, 30f / 255f, 30f / 255f));

        Stroke(new Color(0f, 1f, 0f));
        Line(noteLaneX, 0, noteLaneX, Height);

        NoStroke();
        Fill(new Color(0f, 1f, 0f));
        Circle(noteLaneX, MouseY, 10);

        foreach (NoteState note in notes)
        {
            note.X -= speed;

            Fill(new Color(1f, 200f / 255f, 0f));
            Circle(note.X, note.Y, 10);

            if (!note.Hit && Mathf.Abs(note.X - noteLaneX) < 10 && Mathf.Abs(note.Y - MouseY) < 20)
            {
                note.Hit = true;
                score += 10;
            }
        }

        notes.RemoveAll(note => note.X <= -50);

        DrawSketchString(
            ThemeDB.FallbackFont,
            new Vector2(Width * 0.5f, 30),
            $"Score: {score}",
            HorizontalAlignment.Center,
            -1,
            20,
            Colors.White
        );

        DrawSketchString(
            ThemeDB.FallbackFont,
            new Vector2(Width * 0.5f, Height - 20),
            "Move mouse up/down to match pitch",
            HorizontalAlignment.Center,
            -1,
            14,
            Colors.White
        );
    }
}