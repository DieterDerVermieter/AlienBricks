using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    public class BallController : MonoBehaviour
    {
        [Tooltip("Gets set automatically to the size of the incoming ball data.")]
        [SerializeField] private Transform m_visualsContainer;

        [SerializeField] private SpriteRenderer m_ballSpriteRenderer;


        private static RaycastHit2D[] m_raycastHits = new RaycastHit2D[20];


        private BallData m_data;

        private HashSet<ITriggerHandler> m_triggeredTriggers = new HashSet<ITriggerHandler>();

        private bool m_isActive;


        public Vector3 MovementDirection = Vector2.up;

        public int Damage => m_data.BallDamage;


        public void Setup(BallData data, Vector3 direction)
        {
            m_data = data;
            MovementDirection = direction;

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
            if (!m_isActive)
                return;

            float distanceLeft = GameValues.BallSpeed * Time.fixedDeltaTime;
            float distance = distanceLeft;

            var oldDireciotn = MovementDirection;

            IImpactHandler nextImpactHandler = null;

            var hadImpact = false;
            Vector3 impactPosition = transform.position;

            for (int i = 0; i < Physics2D.CircleCastNonAlloc(transform.position, m_data.BallRadius, MovementDirection, m_raycastHits, distance); i++)
            {
                var hit = m_raycastHits[i];

                var dot = Vector2.Dot(MovementDirection, hit.normal);

                if (hit.collider.isTrigger)
                {
                    var trigger = hit.collider.GetComponent<ITriggerHandler>();
                    if (trigger != null)
                    {
                        if(!m_triggeredTriggers.Contains(trigger) && dot < 0)
                        {
                            m_triggeredTriggers.Add(trigger);
                            trigger.OnBallEnter(this);
                        }

                        trigger.OnBallStay(this);
                    }
                }
                else if (dot < 0 && hit.distance < distance)
                {
                    distance = hit.distance - 0.01f;
                    MovementDirection = Vector2.Reflect(oldDireciotn, hit.normal);

                    nextImpactHandler = hit.collider.GetComponent<IImpactHandler>();

                    hadImpact = true;
                    impactPosition = hit.point;

                    m_triggeredTriggers.Clear();
                }
            }

            transform.position += MovementDirection * distance;

            if (nextImpactHandler != null)
                nextImpactHandler.HandleImpact(this);

            if (hadImpact)
            {
                Instantiate(m_data.BallImpactEffectPrefab, impactPosition, Quaternion.identity);
                SoundEffectManager.Instance.PlayOnce(m_data.BallImpactSound);
            }
        }
    }
}
