using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXTrigger : MonoBehaviour, ITriggerHandler
{
    static RaycastHit2D[] m_raycastHits = new RaycastHit2D[20];


    [SerializeField] GameObject m_vfxPrefab;

    public void Trigger(BallController ball)
    {
        Instantiate(m_vfxPrefab, transform.position, Quaternion.identity);

        for (int i = 0; i < Physics2D.RaycastNonAlloc(transform.position, Vector2.right, m_raycastHits); i++)
        {
            HandleHit(ball, ref m_raycastHits[i]);
        }

        for (int i = 0; i < Physics2D.RaycastNonAlloc(transform.position, Vector2.left, m_raycastHits); i++)
        {
            HandleHit(ball, ref m_raycastHits[i]);
        }
    }


    private void HandleHit(BallController ball, ref RaycastHit2D hit)
    {
        if (hit.collider.isTrigger)
            return;

        var impactHandler = hit.collider.GetComponent<IImpactHandler>();
        if(impactHandler != null)
        {
            impactHandler.HandleImpact(ball);
        }
    }
}
