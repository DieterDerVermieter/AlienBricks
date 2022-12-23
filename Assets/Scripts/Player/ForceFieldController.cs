using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    public class ForceFieldController : MonoBehaviour, ITriggerHandler
    {
        [Header("Settings")]
        [SerializeField] private float m_duration = 5.0f;

        [SerializeField] private float m_strength = 10.0f;
        [SerializeField] private float m_radius = 2.5f;

        [SerializeField] private SoundEffectData m_sound;
        [SerializeField] private float m_soundRetriggerTime;


        private float m_lifetime;
        private float m_soundTimer;


        private void Update()
        {
            m_soundTimer -= Time.deltaTime;
            if(m_soundTimer <= 0)
            {
                m_soundTimer = m_soundRetriggerTime;
                SoundEffectManager.Instance.PlayOnce(m_sound);
            }

            m_lifetime += Time.deltaTime;
            if(m_lifetime >= m_duration)
            {
                Destroy(gameObject);
            }
        }


        public void OnBallEnter(BallController ball) { }

        public void OnBallStay(BallController ball)
        {
            var vectorToBall = ball.transform.position - transform.position;

            var sqrtDistance = vectorToBall.sqrMagnitude;
            var influence = 1 - Mathf.Max(0, sqrtDistance) / (m_radius * m_radius);

            var direction = -vectorToBall.normalized;

            ball.MovementDirection = (ball.MovementDirection + direction * influence * m_strength).normalized;
        }
    }
}
