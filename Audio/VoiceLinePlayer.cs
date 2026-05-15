using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace ProjectPlantris.Audio;

public partial class VoiceLinePlayer : AudioStreamPlayer
{
    [Export] public Array<AudioStream> VoiceLines { get; set; } = [];   
    
    
    
}