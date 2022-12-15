using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace DieterDerVermieter
{
    public class BrickController : BoardObject, IImpactHandler
    {
        [SerializeField] float m_healthPerLevel = 1.0f;
        [SerializeField] int m_pointsPerLevel = 100;

        [SerializeField] int m_stepsPerMove = 1;

        [SerializeField] SpriteRenderer m_healthTintRenderer;
        [SerializeField] TMP_Text m_healthText;

        [SerializeField] Color m_healthTintFull = Color.green;
        [SerializeField] Color m_healthTintDead = Color.red;


        int m_maxHealth;
        int m_currentHealth;


        public int PointsWorth { get; private set; }


        protected override void Update()
        {
            base.Update();

            m_healthText.text = $"{m_currentHealth}";

            var percent = (float)m_currentHealth / m_maxHealth;
            m_healthTintRenderer.color = Color.Lerp(m_healthTintDead, m_healthTintFull, percent);
        }


        public override void Setup(BoardController controller, Vector2Int position)
        {
            base.Setup(controller, position);

            m_maxHealth = (int)(m_boardController.CurrentLevel * m_healthPerLevel);
            m_currentHealth = m_maxHealth;

            PointsWorth = m_pointsPerLevel * controller.CurrentLevel;
        }


        public override void DoMove()
        {
            var steps = m_stepsPerMove;

            var objectsAhead = m_boardController.Board
                .Where(obj => obj.GridPosition.x == GridPosition.x)
                .Where(obj => obj.GridPosition.y > GridPosition.y);

            if (objectsAhead.Count() > 0)
            {
                var nextBlockedRow = objectsAhead.Min(obj => obj.GridPosition.y);
                var maxSteps = nextBlockedRow - GridPosition.y - 1;

                if (maxSteps < steps)
                    steps = maxSteps;
            }

            var targetPosition = GridPosition + Vector2Int.up * steps;
            SetPosition(targetPosition);
        }


        public void HandleImpact(BallController ball)
        {
            m_currentHealth -= ball.Damage;
            if (m_currentHealth <= 0)
            {
                m_currentHealth = 0;
                m_boardController.DestroyObject(this);
            }
        }
    }
}
