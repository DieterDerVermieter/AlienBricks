using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] float m_speedBonusPerImpact = 1.01f;
    [SerializeField] GameObject m_impactEffectPrefab;


    static RaycastHit2D[] m_raycastHits = new RaycastHit2D[20];


    PlayerController m_playerController;

    Vector3 m_movementDirection = Vector2.up;

    HashSet<ITriggerHandler> m_triggeredTriggers = new HashSet<ITriggerHandler>();


    float m_radius = 0.2f;
    float m_speed = 5.0f;

    public bool IsActive = true;

    public int Damage => m_playerController.Combo + 1;


    public void Setup(PlayerController controller, Vector3 direction, float radius, float speed)
    {
        m_playerController = controller;
        m_movementDirection = direction;
        m_radius = radius;
        m_speed = speed;
    }


    private void FixedUpdate()
    {
        if (!IsActive)
            return;

        //var speed = Speed * Mathf.Pow(m_speedBonusPerImpact, m_impactCount);
        float distanceLeft = m_speed * Time.fixedDeltaTime;
        float distance = distanceLeft;

        var nextDirection = m_movementDirection;
        IImpactHandler nextImpactHandler = null;

        for (int i = 0; i < Physics2D.CircleCastNonAlloc(transform.position, m_radius, m_movementDirection, m_raycastHits, distance); i++)
        {
            var hit = m_raycastHits[i];

            var dot = Vector2.Dot(m_movementDirection, hit.normal);
            if (dot > 0)
                continue;

            if(hit.collider.isTrigger)
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
        distanceLeft -= distance;

        m_movementDirection = nextDirection;

        if (nextImpactHandler != null)
            nextImpactHandler.HandleImpact(this);
    }
}
