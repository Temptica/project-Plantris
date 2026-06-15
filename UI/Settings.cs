using Godot;
using ProjectPlantris.Player;
using Slider = ProjectPlantris.UI.Components.SliderComponent.Slider;

namespace ProjectPlantris.UI;

public partial class Settings : Control
{
    private PlayerSettings _settings = null!;

    private Slider _masterVolumeSlider = null!;
    private Slider _musicVolumeSlider = null!;
    private Slider _sfxVolumeSlider = null!;
    private Slider _voicesSlider = null!;

    public override void _Ready()
    {
        _settings = PlayerSettingsManager.GetSettings();
        var returnButton = GetNode<Button>("%ReturnButton");
        returnButton.Pressed += OnReturnButtonPressed;

        _masterVolumeSlider = GetNode<Slider>("%MasterSlider");
        _sfxVolumeSlider = GetNode<Slider>("%SfxSlider");
        _musicVolumeSlider = GetNode<Slider>("%MusicSlider");
        _voicesSlider = GetNode<Slider>("%VoicesSlider");

        SetAudio();
    }

    private void SetAudio()
    {
        var masterSlider = GetNode<Slider>("%MasterSlider");
        var masterVolume = AudioServer.GetBusVolumeLinear(0) * 100;
        masterSlider.SetValue(masterVolume);
        masterSlider.ValueChanged += OnMasterAudioSliderChanged;

        var sfxSlider = GetNode<Slider>("%SfxSlider");
        var sfxVolume = AudioServer.GetBusVolumeLinear(1) * 100;
        sfxSlider.SetValue(sfxVolume);
        sfxSlider.ValueChanged += OnSfxAudioSliderChanged;

        var musicSlider = GetNode<Slider>("%MusicSlider");
        var musicVolume = AudioServer.GetBusVolumeLinear(2) * 100;
        musicSlider.SetValue(musicVolume);
        musicSlider.ValueChanged += OnMusicAudioSliderChanged;

        var voicesSlider = GetNode<Slider>("%VoicesSlider");
        var voicesVolume = AudioServer.GetBusVolumeLinear(3) * 100;
        voicesSlider.SetValue(voicesVolume);
        voicesSlider.ValueChanged += OnVoicesSliderChanged;
    }

    #region Audio

    private void OnMasterAudioSliderChanged(double value)
    {
        _settings.MasterVolume = (float)value;
        UpdateAudioBus(0, value);
    }

    private void OnSfxAudioSliderChanged(double value)
    {
        _settings.SfxVolume = (float)value;
        UpdateAudioBus(1, value);
    }
    private void OnMusicAudioSliderChanged(double value)
    {
        _settings.MusicVolume = (float)value;
        UpdateAudioBus(2, value);
    }


    private void OnVoicesSliderChanged(double value)
    {
        _settings.VoicesVolume = (float)value;
        UpdateAudioBus(3, value);
    }

    private static void UpdateAudioBus(int i, double value)
    {
        if (value <= 0)
        {
            AudioServer.SetBusMute(i, true);

            return;
        }

        if (AudioServer.IsBusMute(i))
        {
            AudioServer.SetBusMute(i, false);
        }

        AudioServer.SetBusVolumeLinear(i, (float)value / 100f);
    }

    #endregion

    private void OnReturnButtonPressed()
    {
        //Save
        PlayerSettingsManager.SaveSettings(_settings);
        
        QueueFree();
    }
}