using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DieterDerVermieter
{
    public class ExtraBallsTrigger : BoardObject, ITriggerHandler
    {
        [SerializeField] int m_extraBalls = 1;


        bool m_triggered = false;


        public void Trigger(BallController ball)
        {
            if (m_triggered)
                return;

            m_triggered = true;

            //PlayerController.BallCount += m_extraBalls;
            m_boardController.DestroyObject(this);
        }


        public override void DoMove()
        {
            var targetPosition = GridPosition + Vector2Int.up;
            var targetPositionBlocked = m_boardController.Board.Any(obj => obj.GridPosition == targetPosition);

            foreach (var obj in m_boardController.Board.Where(obj => obj.GridPosition == targetPosition))
            {
                Debug.Log($"{name}: Move blocked by {obj.name}");
            }

            if (!targetPositionBlocked)
                SetPosition(targetPosition);
        }
    }
}
