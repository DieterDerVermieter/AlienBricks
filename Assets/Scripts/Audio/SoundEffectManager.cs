using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DieterDerVermieter
{
    /// <summary>
    /// Plays <see cref="AudioClip"/>s and manages them, so they don't overload the game sound.
    /// </summary>
    public class SoundEffectManager : MonoBehaviour
    {
        public static SoundEffectManager Instance { get; private set; }


        [SerializeField] private AudioSource m_audioSourcePrefab;
        [SerializeField] private int m_audioSourceCount;
        [SerializeField] private float m_audioClipOverloadThreshold = 1.0f;


        private class ClipInfo
        {
            public AudioSource Source;
            public float Value;
        }


        private AudioSource[] m_sources;
        private Stack<AudioSource> m_freeSources = new Stack<AudioSource>();
        private Dictionary<AudioClip, ClipInfo> m_playingSources = new Dictionary<AudioClip, ClipInfo>();

        private bool m_isMuted = false;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize some sources to get used for playing clips
            m_sources = new AudioSource[m_audioSourceCount];
            for (int i = 0; i < m_audioSourceCount; i++)
            {
                var source = Instantiate(m_audioSourcePrefab, transform);

                m_sources[i] = source;
                m_freeSources.Push(source);
            }

            OnSoundEffectSettingsChanged();
        }


        private void OnEnable()
        {
            // Subscribe to changes in the soundEffect settings
            SettingsManager.OnSoundEffectSettingsChanged += OnSoundEffectSettingsChanged;
        }

        private void OnDisable()
        {
            // Unsubscribe from settings
            SettingsManager.OnSoundEffectSettingsChanged -= OnSoundEffectSettingsChanged;
        }


        /// <summary>
        /// Play an <see cref="AudioClip"/> a single time.
        /// </summary>
        /// <param name="clip">The clip to play.</param>
        public void PlayAudioClip(AudioClip clip)
        {
            // Skip playing audio, if we are muted
            if (m_isMuted)
                return;

            if (m_playingSources.TryGetValue(clip, out var clipInfo))
            {
                // Don't play this clip, if it was played a moment ago
                if (clipInfo.Value >= m_audioClipOverloadThreshold)
                    return;

                clipInfo.Source.PlayOneShot(clip);
                clipInfo.Value += 1;
            }
            else
            {
                // Check, if there is a free source
                if (m_freeSources.Count <= 0)
                    return;

                clipInfo = new ClipInfo();
                clipInfo.Source = m_freeSources.Pop();

                clipInfo.Source.PlayOneShot(clip);
                clipInfo.Value += 1;

                // Put source into playing dict
                m_playingSources.Add(clip, clipInfo);
            }
        }


        private void Update()
        {
            // Go trough playing sources and check, if some are done playing
            foreach (var clip in m_playingSources.Keys.ToList())
            {
                var clipInfo = m_playingSources[clip];
                if (clipInfo.Source.isPlaying)
                {
                    // Adjust the sources value to stop clips from being played to frequent
                    clipInfo.Value *= 1 - Time.deltaTime / clip.length;
                    continue;
                }

                // Move source to free list, so it can get used for another clip
                m_playingSources.Remove(clip);
                m_freeSources.Push(clipInfo.Source);
            }
        }


        private void OnSoundEffectSettingsChanged()
        {
            // Cache, if soundEffects are muted
            m_isMuted = !SettingsManager.SoundEffectsEnabled;

            // Set values for all audioSources
            for (int i = 0; i < m_sources.Length; i++)
            {
                var source = m_sources[i];

                source.mute = m_isMuted;
                source.volume = SettingsManager.SoundEffectVolume;
            }
        }
    }
}
