using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    public class BallController : MonoBehaviour
    {
        [Tooltip("Gets automatically to the size of the incoming ball data.")]
        [SerializeField] private Transform m_visualsContainer;

        [SerializeField] private SpriteRenderer m_ballSpriteRenderer;


        private static RaycastHit2D[] m_raycastHits = new RaycastHit2D[20];


        private BallData m_data;

        private Vector3 m_movementDirection = Vector2.up;

        private HashSet<ITriggerHandler> m_triggeredTriggers = new HashSet<ITriggerHandler>();

        private bool m_isActive;


        public int Damage => m_data.BallDamage;


        public void Setup(BallData data, Vector3 direction)
        {
            m_data = data;
            m_movementDirection = direction;

            m_visualsContainer.localScale = Vector3.one * m_data.BallRadius * 2.0f;
            m_ballSpriteRenderer.sprite = m_data.BallSprite;

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
                float distanceLeft = GameValues.BallSpeed * Time.fixedDeltaTime;
                float distance = distanceLeft;

                var nextDirection = m_movementDirection;

                IImpactHandler nextImpactHandler = null;

                var hadImpact = false;
                Vector3 impactPosition = transform.position;

                for (int i = 0; i < Physics2D.CircleCastNonAlloc(transform.position, m_data.BallRadius, m_movementDirection, m_raycastHits, distance); i++)
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
                        distance = hit.distance - 0.01f;
                        nextDirection = Vector2.Reflect(m_movementDirection, hit.normal);

                        nextImpactHandler = hit.collider.GetComponent<IImpactHandler>();

                        hadImpact = true;
                        impactPosition = hit.point;

                        m_triggeredTriggers.Clear();
                    }
                }

                transform.position += m_movementDirection * distance;
                m_movementDirection = nextDirection;

                if (nextImpactHandler != null)
                    nextImpactHandler.HandleImpact(this);

                if (hadImpact)
                {
                    Instantiate(m_data.BallImpactEffectPrefab, impactPosition, Quaternion.identity);
                    AudioManager.Instance.PlayAudioClip(m_data.BallImpactSound);
                }
            }
        }
    }
}
