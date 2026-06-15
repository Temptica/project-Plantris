using Godot;
using Godot.Collections;

namespace ProjectPlantris.Player;

public partial class PlayerSettings : Resource
{
    #region Audio

    [Export]
    public float MasterVolume
    {
        get;
        set
        {
            field = value;
            EmitChanged();
        }
    } = AudioServer.GetBusVolumeLinear(0) * 100;

    [Export]
    public float MusicVolume
    {
        get;
        set
        {
            field = value;
            EmitChanged();
        }
    } = AudioServer.GetBusVolumeLinear(2) * 100;

    [Export]
    public float SfxVolume
    {
        get;
        set
        {
            field = value;
            EmitChanged();
        }
    } = AudioServer.GetBusVolumeLinear(1) * 100;
    
    [Export]
    public float VoicesVolume
    {
        get;
        set
        {
            field = value;
            EmitChanged();
        }
    } = AudioServer.GetBusVolumeLinear(3) * 100;

    #endregion

    public void Save()
    {
        EmitChanged();
    }
}