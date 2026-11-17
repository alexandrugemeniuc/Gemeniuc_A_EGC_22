using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Egc; // <--- IMPORTANT: Asigură accesul la Mesh, Actions, Scene etc.

namespace Egc.Teme;

public class RoataPanoramica : Scene
{
    public override string Name => "Roata Panoramică";

    private SceneObject _wheelPivot;
    private readonly List<SceneObject> _cabins = new();

    private const float BaseSpeed = 45.0f;
    private float _currentSpeedMultiplier = 1.0f;
    private Tween _speedTween;
    private bool _isReversed = false;

    public const string Accelerate = "accelerează";
    public const string Decelerate = "încetinește";
    public const string Reverse = "inversează sensul";
    public const string Reset = "resetează viteza";

    public override IEnumerable<string> ActionNames => new[]
    {
        Actions.Quit, Actions.Pause, Actions.Info,
        Accelerate, Decelerate, Reverse, Reset
    };

    public override void Load(Input input)
    {
        input.Bind(Accelerate, Keys.Up, Keys.W);
        input.Bind(Decelerate, Keys.Down, Keys.S);
        input.Bind(Reverse, Keys.Space);
        input.Bind(Reset, Keys.R);

        // ⚠️ ATENȚIE: Dacă aici apare eroare cu "Mesh", schimbă în "Meshes" sau "Primitives"
        // exact cum este scris în fișierul tău LabScene.cs
        
        var pole = new SceneObject("Stâlp", Meshes.Cylinder, null)
        {
            Transform = { Position = new Vector3(0, 2.5f, 0), Scale = new Vector3(0.2f, 5.0f, 0.2f) },
            Tint = new Color4(0.4f, 0.4f, 0.4f, 1.0f)
        };
        Objects.Add(pole); // <--- Dacă dă eroare, încearcă Add(pole);

        _wheelPivot = new SceneObject("PivotRoată", null, pole)
        {
            Transform = { Position = new Vector3(0, 5.0f, 0) }
        };
        Objects.Add(_wheelPivot);

        var rim = new SceneObject("Jantă", Meshes.Cylinder, _wheelPivot)
        {
            Transform = { Scale = new Vector3(3.0f, 3.0f, 0.1f) },
            Tint = new Color4(0.8f, 0.2f, 0.2f, 1.0f)
        };
        Objects.Add(rim);

        float radius = 3.0f;
        _cabins.Add(CreateCabin("Cabină 1", new Vector3(radius, 0, 0), new Color4(0.2f, 0.6f, 1.0f, 1.0f)));
        _cabins.Add(CreateCabin("Cabină 2", new Vector3(-radius, 0, 0), new Color4(1.0f, 0.6f, 0.2f, 1.0f)));
        _cabins.Add(CreateCabin("Cabină 3", new Vector3(0, radius, 0), new Color4(0.2f, 1.0f, 0.4f, 1.0f)));
        _cabins.Add(CreateCabin("Cabină 4", new Vector3(0, -radius, 0), new Color4(1.0f, 0.2f, 0.6f, 1.0f)));

        _speedTween = new Tween(
            from: new Vector3(1.0f, 0, 0),
            to: new Vector3(1.0f, 0, 0),
            duration: 1.0f,
            ease: Easing.EaseInOut
        );
    }

    private SceneObject CreateCabin(string name, Vector3 localPos, Color4 color)
    {
        var cabin = new SceneObject(name, Meshes.Box, _wheelPivot)
        {
            Transform = { Position = localPos, Scale = new Vector3(0.8f, 1.2f, 0.8f) },
            Tint = color
        };
        Objects.Add(cabin);
        return cabin;
    }

    public override void Update(float dt, Input input)
    {
        if (!_speedTween.IsFinished)
        {
            _speedTween.Update(dt);
            _currentSpeedMultiplier = _speedTween.Value.X;
        }

        if (input.WasPressed(Accelerate))
        {
            float target = _isReversed ? -2.5f : 2.5f;
            _speedTween.GoTo(new Vector3(target, 0, 0), 1.0f);
        }
        else if (input.WasPressed(Decelerate))
        {
            float target = _isReversed ? -0.2f : 0.2f;
            _speedTween.GoTo(new Vector3(target, 0, 0), 1.0f);
        }
        else if (input.WasPressed(Reverse))
        {
            _isReversed = !_isReversed;
            float target = _isReversed ? -1.0f : 1.0f;
            _speedTween.GoTo(new Vector3(target, 0, 0), 1.5f);
        }
        else if (input.WasPressed(Reset))
        {
            _isReversed = false;
            _speedTween.GoTo(new Vector3(1.0f, 0, 0), 1.0f);
        }

        float currentSpeed = BaseSpeed * _currentSpeedMultiplier;
        _wheelPivot.Spin = new Vector3(0, currentSpeed, 0);
        _wheelPivot.Update(dt);

        float wheelRotationY = _wheelPivot.Transform.Rotation.Y;
        foreach (var cabin in _cabins)
        {
            cabin.Transform.Rotation = new Vector3(0, -wheelRotationY, 0);
        }

        base.Update(dt, input);
    }

    public override string Status => 
        $"Viteză: {_currentSpeedMultiplier:F1}x | " +
        $"Sens: {(_isReversed ? "Orar" : "Trigonometric")} | " +
        $"Stare: {(_speedTween.IsFinished ? "Constantă" : "În accelerație/frânare")}";
}