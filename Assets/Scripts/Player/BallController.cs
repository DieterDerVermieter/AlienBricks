using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    public class BallController : MonoBehaviour
    {
        [SerializeField] private float m_speedBonusPerImpact = 1.01f;
        [SerializeField] private GameObject m_impactEffectPrefab;

        [SerializeField] private float m_speed = 5.0f;
        [SerializeField] private float m_radius = 0.2f;

        [SerializeField] private AudioClip m_impactSound;


        private static RaycastHit2D[] m_raycastHits = new RaycastHit2D[20];


        private Vector3 m_movementDirection = Vector2.up;

        private HashSet<ITriggerHandler> m_triggeredTriggers = new HashSet<ITriggerHandler>();

        private bool m_isActive;


        public int Damage => 1;


        public void Setup( Vector3 direction)
        {
            m_movementDirection = direction;
            m_isActive = true;
        }


        public void Deactivate()
        {
            m_isActive = false;
        }


        private void FixedUpdate()
        {
            if(m_isActive)
            {
                //var speed = Speed * Mathf.Pow(m_speedBonusPerImpact, m_impactCount);
                float distanceLeft = m_speed * Time.fixedDeltaTime;
                float distance = distanceLeft;

                var nextDirection = m_movementDirection;
                IImpactHandler nextImpactHandler = null;
                var hadImpact = false;

                for (int i = 0; i < Physics2D.CircleCastNonAlloc(transform.position, m_radius, m_movementDirection, m_raycastHits, distance); i++)
                {
                    var hit = m_raycastHits[i];

                    var dot = Vector2.Dot(m_movementDirection, hit.normal);
                    if (dot > 0)
                        continue;

                    if (hit.collider.isTrigger)
                    {
                        var trigger = hit.collider.GetComponent<ITriggerHandler>();
                        if (trigger != null && !m_triggeredTriggers.Contains(trigger))
                        {
                            m_triggeredTriggers.Add(trigger);
                            trigger.Trigger(this);
                        }
                    }
                    else if (hit.distance < distance)
                    {
                        distance = hit.distance;
                        nextDirection = m_movementDirection;
                        nextImpactHandler = hit.collider.GetComponent<IImpactHandler>();
                        m_triggeredTriggers.Clear();
                        hadImpact = true;

                        Instantiate(m_impactEffectPrefab, hit.point, Quaternion.identity);

                        if (Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.y))
                        {
                            nextDirection.x = -nextDirection.x;
                        }
                        else
                        {
                            nextDirection.y = -nextDirection.y;
                        }
                    }
                }

                transform.position += m_movementDirection * distance;
                m_movementDirection = nextDirection;

                if(hadImpact)
                    AudioManager.Instance.PlayAudioClip(m_impactSound);

                if (nextImpactHandler != null)
                    nextImpactHandler.HandleImpact(this);
            }
        }
    }
}
