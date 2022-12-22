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
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }


        [SerializeField] private AudioSource m_audioSourcePrefab;
        [SerializeField] private int m_audioSourceCount;
        [SerializeField] private float m_audioClipOverloadThreshold = 1.0f;


        private class ClipInfo
        {
            public AudioSource Source;
            public float Value;
        }


        private List<AudioSource> m_freeSources = new List<AudioSource>();
        private Dictionary<AudioClip, ClipInfo> m_playingSources = new Dictionary<AudioClip, ClipInfo>();


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
            for (int i = 0; i < m_audioSourceCount; i++)
            {
                var source = Instantiate(m_audioSourcePrefab, transform);
                m_freeSources.Add(source);
            }
        }


        /// <summary>
        /// Play an <see cref="AudioClip"/> a single time.
        /// </summary>
        /// <param name="clip">The clip to play.</param>
        public void PlayAudioClip(AudioClip clip)
        {
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
                clipInfo.Source = m_freeSources[0];

                clipInfo.Source.PlayOneShot(clip);
                clipInfo.Value += 1;

                // Put source from free list to playing dict
                m_freeSources.RemoveAt(0);
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
                m_freeSources.Add(clipInfo.Source);
            }
        }
    }
}
