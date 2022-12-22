using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }


        public static bool SoundEffectsEnabled { get; private set; } = true;
        public static float SoundEffectVolume { get; private set; } = 0.5f;

        public static System.Action OnSoundEffectSettingsChanged;

        public static bool MusicEnabled { get; private set; } = true;
        public static float MusicVolume { get; private set; } = 0.5f;

        public static System.Action OnMusicSettingsChanged;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }


        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Save();
            }
            else
            {
                Load();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }


        private void Save()
        {
            Debug.Log($"Save Settings");

            PlayerPrefs.SetInt("soundEffectsEnabled", SoundEffectsEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("soundEffectVolume", SoundEffectVolume);

            PlayerPrefs.SetInt("musicEnabled", MusicEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("musicVolume", MusicVolume);

            PlayerPrefs.Save();
        }

        private void Load()
        {
            Debug.Log($"Load Settings");

            if (!PlayerPrefs.HasKey("soundSettings"))
            {
                PlayerPrefs.SetInt("soundSettings", 0);
                Save();
            }

            var soundEffectsEnabled = PlayerPrefs.GetInt("soundEffectsEnabled") > 0;
            var soundEffectVolume = PlayerPrefs.GetFloat("soundEffectVolume");

            var musicEnabled = PlayerPrefs.GetInt("musicEnabled") > 0;
            var musicVolume = PlayerPrefs.GetFloat("musicVolume");

            SetSoundEffectSettings(soundEffectsEnabled, soundEffectVolume);
            SetMusicSettings(musicEnabled, musicVolume);
        }


        public static void SetSoundEffectSettings(bool enabled, float volume)
        {
            SoundEffectsEnabled = enabled;
            SoundEffectVolume = volume;

            OnSoundEffectSettingsChanged?.Invoke();
        }

        public static void SetMusicSettings(bool enabled, float volume)
        {
            MusicEnabled = enabled;
            MusicVolume = volume;

            OnMusicSettingsChanged?.Invoke();
        }
    }
}
