using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DieterDerVermieter
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource m_audioSourcePrefab;
        [SerializeField] private int m_audioSourceCount;


        private static List<AudioSource> m_freeSources = new List<AudioSource>();
        private static Dictionary<AudioClip, AudioSource> m_playingSources = new Dictionary<AudioClip, AudioSource>();

        private static AudioSource[] m_audioSources;
        private static int m_nextFreeSource;


        private void Awake()
        {
            //m_audioSources = new AudioSource[m_audioSourceCount];
            for (int i = 0; i < m_audioSourceCount; i++)
            {
                var source = Instantiate(m_audioSourcePrefab, transform);
                //m_audioSources[i] = source;
                m_freeSources.Add(source);
            }
        }


        public static void PlayAudioClip(AudioClip clip)
        {
            if(!m_playingSources.TryGetValue(clip, out var source))
            {
                if (m_freeSources.Count <= 0)
                    return;

                source = m_freeSources[0];

                m_freeSources.RemoveAt(0);
                m_playingSources.Add(clip, source);
            }

            source.PlayOneShot(clip);


            /*
            if (m_nextFreeSource >= m_audioSources.Length)
                return;

            m_audioSources[m_nextFreeSource].PlayOneShot(clip);
            m_nextFreeSource++;
            */
        }


        private void Update()
        {
            foreach (var clip in m_playingSources.Keys.ToList())
            {
                var source = m_playingSources[clip];
                if (source.isPlaying)
                {
                    continue;
                }

                m_playingSources.Remove(clip);
                m_freeSources.Add(source);
            }

            /*
            int i = 0;
            while(i < m_nextFreeSource)
            {
                if (m_audioSources[i].isPlaying)
                {
                    i++;
                    continue;
                }

                m_nextFreeSource--;

                var tmp = m_audioSources[m_nextFreeSource];
                m_audioSources[m_nextFreeSource] = m_audioSources[i];
                m_audioSources[i] = tmp;
            }
            */
        }
    }
}
