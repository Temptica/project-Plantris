using System;
using Godot;
using ProjectPlantris.Managers;

namespace ProjectPlantris.Audio;

public partial class AudioPlayer : AudioStreamPlayer
{
    [Export] private AudioStream _plopSound;
    public override void _Ready()
    {
        MovementController.Instance.FlowerPlaced += _ =>
        {
            PitchScale = Random.Shared.NextSingle() * 0.2f + 0.9f; //(0.9, 1.1)
            SetStream(_plopSound);
            Play();
        };
    }
}