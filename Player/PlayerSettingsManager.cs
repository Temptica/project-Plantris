using Godot;

namespace ProjectPlantris.Player;

public static class PlayerSettingsManager
{
    private const string SettingsPath = "user://settings.res";
    private static PlayerSettings? _settingsCache;
    private static SceneTree GetTree() => (SceneTree)Engine.GetMainLoop();
    private static SceneTreeTimer? Timer;

    public static PlayerSettings GetSettings()
    {
        if (_settingsCache is not null) return _settingsCache;

        _settingsCache?.Changed -= OnSettingsChanged;

        if (!FileAccess.FileExists(SettingsPath))
        {
            _settingsCache = new PlayerSettings();
            _settingsCache.Changed += OnSettingsChanged;
            SaveSettings();
            return _settingsCache;
        }

        _settingsCache = ResourceLoader.Load<PlayerSettings>(SettingsPath);

        AudioServer.SetBusVolumeLinear(0, _settingsCache.MasterVolume / 100f);
        AudioServer.SetBusVolumeLinear(1, _settingsCache.SfxVolume / 100f);
        AudioServer.SetBusVolumeLinear(2, _settingsCache.MusicVolume / 100f);
        AudioServer.SetBusVolumeLinear(3, _settingsCache.VoicesVolume / 100f);

        _settingsCache.Changed += OnSettingsChanged;
        return _settingsCache;
    }

    public static void SaveSettings(PlayerSettings? settings = null)
    {
        settings ??= _settingsCache;

        if (Timer is null)
        {
            Timer = GetTree().CreateTimer(1f);
            Timer.Timeout += () => OnSaveSettings(settings);
            return;
        }

        Timer.SetTimeLeft(1f);
    }

    private static void OnSaveSettings(PlayerSettings? settings)
    {
        Timer = null;

        GD.Print("Saving settings...");
        var result = ResourceSaver.Save(settings, SettingsPath);
        if (result != Error.Ok)
        {
            GD.PrintErr($"Failed to save settings. Error code: {result}");
        }
    }

    private static void OnSettingsChanged()
    {
        SaveSettings();
    }
}