using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DieterDerVermieter
{
    /// <summary>
    /// Base class for all objects that live on the board grid and should get managed by the <see cref="BoardController"/>.
    /// </summary>
    public abstract class BoardObject : MonoBehaviour
    {
        protected BoardController m_boardController;


        bool m_isMoving;


        public virtual bool IsBusy => m_isMoving;

        public Vector2Int GridPosition { get; private set; }


        public virtual void Setup(BoardController controller, Vector2Int position)
        {
            m_boardController = controller;

            GridPosition = position;
            transform.position = m_boardController.GridToWorldPosition(GridPosition);
        }


        protected virtual void Update()
        {
            if (m_isMoving)
            {
                var targetPosition = m_boardController.GridToWorldPosition(GridPosition);
                var distance = Vector3.Distance(targetPosition, transform.position);
                if (distance > 0.1f)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, 10.0f * Time.deltaTime);
                }
                else
                {
                    transform.position = targetPosition;
                    m_isMoving = false;
                }
            }
        }


        protected void SetPosition(Vector2Int position)
        {
            m_isMoving = true;
            GridPosition = position;
        }


        public abstract void DoMove();
    }
}
