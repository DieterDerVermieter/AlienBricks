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

        private class SourceData
        {
            public AudioSource Source;
            public float TimeLeft;
        }


        private SourceData[] m_sources;
        private int m_nextFreeSource = 0;

        private float[] m_priorityGauge;

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
            m_sources = new SourceData[m_audioSourceCount];
            for (int i = 0; i < m_audioSourceCount; i++)
            {
                var source = Instantiate(m_audioSourcePrefab, transform);
                m_sources[i] = new SourceData { Source = source };
            }

            m_priorityGauge = new float[SoundEffectData.MAX_PRIORITY + 1];

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


        public void PlayOnce(SoundEffectData data)
        {
            if (!TryPlay(data, out var sourceData))
                return;

            sourceData.Source.loop = false;
        }

        public void PlayLooped(SoundEffectData data, float duration)
        {
            if (!TryPlay(data, out var sourceData))
                return;

            sourceData.Source.loop = true;
            sourceData.TimeLeft = duration;
        }

        public void PlayLooped(SoundEffectData data, int count)
        {
            if (!TryPlay(data, out var sourceData))
                return;

            sourceData.Source.loop = true;
            sourceData.TimeLeft = count * data.Clip.length;
        }


        private bool TryPlay(SoundEffectData soundEffectData, out SourceData sourceData)
        {
            sourceData = null;

            if (m_nextFreeSource >= m_sources.Length)
                return false;

            var gauge = m_priorityGauge[soundEffectData.Priority];
            if (gauge > m_audioClipOverloadThreshold)
                return false;

            sourceData = m_sources[m_nextFreeSource];
            m_nextFreeSource++;

            sourceData.Source.clip = soundEffectData.Clip;
            sourceData.Source.priority = soundEffectData.Priority;

            sourceData.Source.Play();

            return true;
        }


        private void Update()
        {
            int index = 0;
            while(index < m_nextFreeSource)
            {
                var sourceData = m_sources[index];

                if(sourceData.Source.loop)
                {
                    sourceData.TimeLeft -= Time.deltaTime;
                    if(sourceData.TimeLeft > 0)
                    {
                        index++;
                        continue;
                    }

                    sourceData.Source.Stop();
                }
                else
                {
                    if(sourceData.Source.isPlaying)
                    {
                        index++;
                        continue;
                    }
                }

                m_nextFreeSource--;

                var tmp = m_sources[index];
                m_sources[index] = m_sources[m_nextFreeSource];
                m_sources[m_nextFreeSource] = tmp;
            }

            for (int i = 0; i < m_priorityGauge.Length; i++)
            {
                m_priorityGauge[i] *= 1 - Time.deltaTime;
            }
        }


        private void OnSoundEffectSettingsChanged()
        {
            // Cache, if soundEffects are muted
            m_isMuted = !SettingsManager.SoundEffectsEnabled;

            // Set values for all audioSources
            for (int i = 0; i < m_sources.Length; i++)
            {
                var sourceData = m_sources[i];

                sourceData.Source.mute = m_isMuted;
                sourceData.Source.volume = SettingsManager.SoundEffectVolume;
            }
        }
    }
}
