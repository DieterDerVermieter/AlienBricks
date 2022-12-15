using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour, IImpactHandler
{
    [SerializeField] Transform m_collectionTarget;
    [SerializeField] float m_collectionSpeed = 1.0f;


    List<BallController> m_balls = new List<BallController>();


    public System.Action<Vector3> OnBallKilledCallback;
    public System.Action OnBallCollectedCallback;


    public void HandleImpact(BallController ball)
    {
        ball.IsActive = false;
        m_balls.Add(ball);
        OnBallKilledCallback?.Invoke(ball.transform.position);
    }


    private void FixedUpdate()
    {
        for (int i = 0; i < m_balls.Count;)
        {
            var ball = m_balls[i];
            ball.transform.position = Vector3.Lerp(ball.transform.position, m_collectionTarget.position, m_collectionSpeed * Time.fixedDeltaTime);

            var sqrtDist = (m_collectionTarget.position - ball.transform.position).sqrMagnitude;
            if(sqrtDist < 0.01f)
            {
                Destroy(ball.gameObject);
                m_balls.RemoveAt(i);
                OnBallCollectedCallback?.Invoke();
                continue;
            }

            i++;
        }
    }
}
