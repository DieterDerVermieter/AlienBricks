using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace DieterDerVermieter
{
    public class SettingsUIController : MonoBehaviour
    {
        [SerializeField] private AudioClip m_volumeTestSound;

        [SerializeField] private Button m_backButton;

        [Header("Sound Effects")]
        [SerializeField] private Toggle m_soundEffectsEnabledToggle;
        [SerializeField] private Slider m_soundEffectVolumeSlider;
        [SerializeField] private TMP_Text m_soundEffectVolumeText;

        [Header("Music")]
        [SerializeField] private Toggle m_musicEnabledToggle;
        [SerializeField] private Slider m_musicVolumeSlider;
        [SerializeField] private TMP_Text m_musicVolumeText;


        private void OnEnable()
        {
            // Setup initial values
            m_soundEffectsEnabledToggle.isOn = SettingsManager.SoundEffectsEnabled;
            m_soundEffectVolumeSlider.value = SettingsManager.SoundEffectVolume * 100;
            m_soundEffectVolumeText.text = $"{m_soundEffectVolumeSlider.value}";

            m_musicEnabledToggle.isOn = SettingsManager.MusicEnabled;
            m_musicVolumeSlider.value = SettingsManager.MusicVolume * 100;
            m_musicVolumeText.text = $"{m_musicVolumeSlider.value}";

            // Add listeners
            m_backButton.onClick.AddListener(BackButtonOnClick);

            m_soundEffectsEnabledToggle.onValueChanged.AddListener(SoundEffectsEnabledToggleOnValueChanged);
            m_soundEffectVolumeSlider.onValueChanged.AddListener(SoundEffectVolumeSliderOnValueChanged);

            m_musicEnabledToggle.onValueChanged.AddListener(MusicEnabledToggleOnValueChanged);
            m_musicVolumeSlider.onValueChanged.AddListener(MusicVolumeSliderOnValueChanged);
        }

        private void OnDisable()
        {
            // Renmove listeners
            m_backButton.onClick.RemoveListener(BackButtonOnClick);

            m_soundEffectsEnabledToggle.onValueChanged.RemoveListener(SoundEffectsEnabledToggleOnValueChanged);
            m_soundEffectVolumeSlider.onValueChanged.RemoveListener(SoundEffectVolumeSliderOnValueChanged);

            m_musicEnabledToggle.onValueChanged.RemoveListener(MusicEnabledToggleOnValueChanged);
            m_musicVolumeSlider.onValueChanged.RemoveListener(MusicVolumeSliderOnValueChanged);
        }


        private void BackButtonOnClick()
        {
            gameObject.SetActive(false);
        }


        private void SoundEffectsEnabledToggleOnValueChanged(bool state)
        {
            SettingsManager.SetSoundEffectSettings(state, SettingsManager.SoundEffectVolume);

            SoundEffectManager.Instance.PlayAudioClip(m_volumeTestSound);
        }

        private void SoundEffectVolumeSliderOnValueChanged(float value)
        {
            m_soundEffectVolumeText.text = $"{value}";

            var volume = value / 100;
            SettingsManager.SetSoundEffectSettings(SettingsManager.SoundEffectsEnabled, volume);

            SoundEffectManager.Instance.PlayAudioClip(m_volumeTestSound);
        }


        private void MusicEnabledToggleOnValueChanged(bool state)
        {
            SettingsManager.SetMusicSettings(state, SettingsManager.MusicVolume);
        }

        private void MusicVolumeSliderOnValueChanged(float value)
        {
            m_musicVolumeText.text = $"{value}";

            var volume = value / 100;
            SettingsManager.SetMusicSettings(SettingsManager.MusicEnabled, volume);
        }
    }
}
