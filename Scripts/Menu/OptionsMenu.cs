using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Dawnshard.Menu;
using Dawnshard.Network;
using FMOD;
using Debug = UnityEngine.Debug;

public class OptionsMenu : Popup
{
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundEffectsVolumeSlider;
    [SerializeField] private Toggle muteToggle;
    

    protected override void Start()
    {
        base.Start();
        // Get the music and sound effects buses from FMOD
        languageDropdown.onValueChanged.AddListener((_) => UpdateLanguage());
        musicVolumeSlider.onValueChanged.AddListener((_) => UpdateMusicVolume());
        soundEffectsVolumeSlider.onValueChanged.AddListener((_) => UpdateSoundEffectsVolume());
        muteToggle.onValueChanged.AddListener((_) => ToggleMute());
        
        RefreshSettings();
#if UNITY_IOS || UNITY_ANDROID
        transform.localScale *= 1.3f;
#endif
    }

    // Call this method to open the options menu
    public override void Open()
    {
        base.Open();
        RefreshSettings();
    }
    

    private void RefreshSettings()
    {
        //TODO: questo non dovrebbe stare qui
        languageDropdown.value = PlayerPrefs.GetInt(Constants.LanguageSetting, 0);
        musicVolumeSlider.value = PlayerPrefs.GetFloat(Constants.MusicSetting, .5f);
        soundEffectsVolumeSlider.value = PlayerPrefs.GetFloat(Constants.SoundEffectsSetting, .5f);
        SetFmodVolume();
        muteToggle.isOn = PlayerPrefs.GetInt(Constants.MuteSetting, 0) == 1;
        RuntimeManager.MuteAllEvents(muteToggle.isOn);
    }

    private void SetFmodVolume()
    {
        Bank[] bankList;
        FMODUnity.RuntimeManager.StudioSystem.getBankList(out bankList);
        var volume = soundEffectsVolumeSlider.value;
        foreach (var bank in bankList)
        {
            bank.getBusList(out var buses);
            foreach (var bus in buses)
            {
                bus.getPath(out var path);
                if (path.Contains(Constants.SFX))
                {
                    bus.setVolume(soundEffectsVolumeSlider.value);
                }
                else if (path.Contains(Constants.Music))
                {
                    bus.setVolume(musicVolumeSlider.value);
                }
            }
        }
    }

    /// <summary>
    /// Call this method to update the selected language
    /// </summary>
    private void UpdateLanguage()
    {
        string selectedLanguage = languageDropdown.options[languageDropdown.value].text;
        // Perform actions based on the selected language
        PlayerPrefs.SetInt(Constants.LanguageSetting, languageDropdown.value);
        Debug.Log("Selected language: " + selectedLanguage);
    }

    /// <summary>
    /// Call this method to update the music volume
    /// </summary>
    private void UpdateMusicVolume()
    {
        float musicVolume = musicVolumeSlider.value;
        // Set the music volume to the desired value using FMOD
        SetFmodVolume();
        PlayerPrefs.SetFloat(Constants.MusicSetting, musicVolume);
        Debug.Log("Music volume: " + musicVolume);
    }

    /// <summary>
    /// Call this method to update the sound effects volume
    /// </summary>
    private void UpdateSoundEffectsVolume()
    {
        float soundEffectsVolume = soundEffectsVolumeSlider.value;
        // Set the sound effects volume to the desired value using FMOD
        SetFmodVolume();
        PlayerPrefs.SetFloat(Constants.SoundEffectsSetting, soundEffectsVolume);
        Debug.Log("Sound effects volume: " + soundEffectsVolume);
    }

    /// <summary>
    /// Call this method to toggle mute/unmute the game
    /// </summary>
    private void ToggleMute()
    {
        bool isMuted = muteToggle.isOn;
        // Set the game mute state based on the toggle value using FMOD
        RuntimeManager.MuteAllEvents(isMuted);
        PlayerPrefs.SetInt(Constants.MuteSetting, isMuted ? 1 : 0);
        Debug.Log("Mute: " + isMuted);
    }
}
