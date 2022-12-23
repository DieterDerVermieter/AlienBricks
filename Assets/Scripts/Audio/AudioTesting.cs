using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    public class AudioTesting : MonoBehaviour
    {
        [SerializeField] private SoundEffectData m_sound;

        [SerializeField] private float m_frequency = 1.0f;
        [SerializeField] private int m_amount = 1;

        [SerializeField] private bool m_looped = false;
        [SerializeField] private int m_loppedCount = 1;


        private float m_timer;


        private void Update()
        {
            m_timer -= Time.deltaTime;
            if(m_timer <= 0)
            {
                m_timer = 1 / m_frequency;
                for (int i = 0; i < m_amount; i++)
                {
                    if(!m_looped)
                        SoundEffectManager.Instance.PlayOnce(m_sound);
                    else
                        SoundEffectManager.Instance.PlayLooped(m_sound, m_loppedCount);
                }
            }
        }
    }
}
