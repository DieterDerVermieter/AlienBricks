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


        private float m_lifetime;


        private void Update()
        {
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
