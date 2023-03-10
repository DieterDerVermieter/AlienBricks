using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DieterDerVermieter
{
    public class ExtraBallsTrigger : BoardObject, ITriggerHandler
    {
        [SerializeField] private int m_extraBalls = 1;

        [SerializeField] private SoundEffectData m_collectionSound;


        private bool m_triggered = false;


        /// <summary>
        /// <inheritdoc/>
        /// Increases the ball count by <see cref="m_extraBalls"/> and destroys itself.
        /// </summary>
        /// <param name="ball"><inheritdoc/></param>
        public void OnBallEnter(BallController ball)
        {
            if (m_triggered)
                return;

            m_triggered = true;

            GameValues.BallCount += m_extraBalls;
            m_boardController.DestroyObject(this);

            SoundEffectManager.Instance.PlayOnce(m_collectionSound);
        }

        public void OnBallStay(BallController ball) { }


        /// <summary>
        /// <inheritdoc/>
        /// Moves one cell down, if it isn't blocked.
        /// </summary>
        public override void DoMove()
        {
            var targetPosition = GridPosition + Vector2Int.up;

            // Check, if targetPosition is blocked
            var targetPositionBlocked = m_boardController.Board.Any(obj => obj.GridPosition == targetPosition);

            if (!targetPositionBlocked)
                SetPosition(targetPosition);
        }
    }
}
