using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] m_musicTracks;
        [SerializeField] private bool m_loop = true;


        private AudioSource m_audioSource;


        private void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();
            m_audioSource.loop = m_loop;

            OnMusicSettingsChanged();
        }


        private void OnEnable()
        {
            SettingsManager.OnMusicSettingsChanged += OnMusicSettingsChanged;
        }

        private void OnDisable()
        {
            SettingsManager.OnMusicSettingsChanged -= OnMusicSettingsChanged;
        }


        private void Start()
        {
            StartRandomTrack();
        }


        private void Update()
        {
            if (m_loop)
                return;

            if (!m_audioSource.isPlaying)
            {
                StartRandomTrack();
            }
        }


        private void StartRandomTrack()
        {
            var track = m_musicTracks[Random.Range(0, m_musicTracks.Length)];

            m_audioSource.clip = track;
            m_audioSource.Play();
        }


        private void OnMusicSettingsChanged()
        {
            m_audioSource.mute = !SettingsManager.MusicEnabled;
            m_audioSource.volume = SettingsManager.MusicVolume;
        }
    }
}
