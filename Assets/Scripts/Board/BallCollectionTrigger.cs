using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    public class BallCollectionTrigger : MonoBehaviour, ITriggerHandler
    {
        [SerializeField] PlayerController m_player;


        List<BallController> m_collectedBalls = new List<BallController>();


        public void OnBallEnter(BallController ball)
        {
            ball.Deactivate();
            m_collectedBalls.Add(ball);

            m_player.OnBallCatched(ball);
        }

        public void OnBallStay(BallController ball) { }


        private void Update()
        {
            var lerpTime = 10.0f * Time.deltaTime;
            var targetPosition = m_player.Ship.position;

            for (int i = 0; i < m_collectedBalls.Count;)
            {
                var ball = m_collectedBalls[i];

                ball.transform.position = Vector3.Lerp(ball.transform.position, targetPosition, lerpTime);

                var distanceToTarget = Vector3.Distance(ball.transform.position, targetPosition);
                if(distanceToTarget <= 0.1f)
                {
                    m_collectedBalls.RemoveAt(i);
                    m_player.OnBallCollected(ball);

                    continue;
                }

                i++;
            }
        }
    }
}
