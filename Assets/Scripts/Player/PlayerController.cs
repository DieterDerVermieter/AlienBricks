using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DieterDerVermieter
{
    public class PlayerController : TurnHandler
    {
        [Header("Aim Indication")]
        [SerializeField] private LineRenderer m_aimLine;

        [Header("Shooting")]
        [SerializeField] private BallController m_ballPrefab;

        [SerializeField] private float m_maxAimAngle = 70.0f;
        [SerializeField] private float m_shootDelay = 0.1f;


        private Camera m_mainCamera;
        private RaycastHit2D[] m_raycastHits = new RaycastHit2D[10];

        private enum State
        {
            Aiming,
            Shooting,
            Collecting
        }

        private State m_currentState;

        private Vector2 m_shootingDirection;
        private int m_shootingCount;

        private int m_shootingCounter;
        private float m_shootingTimer;

        private int m_collectionCounter;

        private bool m_hasNextPosition;
        private Vector3 m_nextPosition;


        private void Awake()
        {
            m_mainCamera = Camera.main;
        }


        private void Update()
        {
            if (IsTurnActive)
            {
                switch (m_currentState)
                {
                    case State.Aiming:
                        {
                            var aimDirection = CalculateAimDirection();
                            UpdateAimIndicator(aimDirection);

                            if (Input.GetButtonDown("Fire1"))
                            {
                                // Start shooting
                                m_currentState = State.Shooting;

                                // Reset relevant values
                                m_shootingDirection = aimDirection;
                                m_shootingCount = GameValues.BallCount;

                                m_shootingCounter = 0;
                                m_shootingTimer = 0;

                                m_collectionCounter = 0;

                                m_hasNextPosition = false;
                            }
                        }
                        break;

                    case State.Shooting:
                        {
                            m_shootingTimer -= Time.deltaTime;
                            if(m_shootingTimer <= 0)
                            {
                                m_shootingTimer += m_shootDelay;
                                m_shootingCounter++;

                                ShootBall(m_shootingDirection);
                            }

                            // Are all balls shoot out
                            if (m_shootingCounter >= m_shootingCount)
                            {
                                // Start collecting
                                m_currentState = State.Collecting;
                            }
                        }
                        break;

                    case State.Collecting:
                        {
                            if(m_hasNextPosition)
                            {
                                // Smoothly move to next position
                                var lerpTime = 10.0f * Time.deltaTime;
                                transform.position = Vector3.Lerp(transform.position, m_nextPosition, lerpTime);
                            }

                            // Are all balls collected
                            if(m_collectionCounter >= m_shootingCount)
                            {
                                // Snap to next position if it wasn't reached
                                transform.position = m_nextPosition;

                                // End turn
                                IsTurnActive = false;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            // Only enable the aim line if we are currently aiming
            m_aimLine.gameObject.SetActive(IsTurnActive && m_currentState == State.Aiming);
        }


        private Vector2 CalculateAimDirection()
        {
            // Caculate aim direction based on mouse position
            Vector2 mousePosition = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 aimDirection = (mousePosition - (Vector2)transform.position).normalized;

            // Calculate aim angle
            var aimAngle = Vector2.SignedAngle(Vector2.up, aimDirection);
            var maxRad = m_maxAimAngle * Mathf.Deg2Rad;

            // Constraint aim direction based on a maximum aim angle
            if (aimAngle > m_maxAimAngle) aimDirection = new Vector2(-Mathf.Sin(maxRad), Mathf.Cos(maxRad));
            if (aimAngle < -m_maxAimAngle) aimDirection = new Vector2(-Mathf.Sin(-maxRad), Mathf.Cos(-maxRad));

            return aimDirection;
        }


        private void UpdateAimIndicator(Vector2 aimDirection)
        {
            // Calculate the closest distance before a ball would collide with something
            var distance = Mathf.Infinity;
            for (int i = 0; i < Physics2D.RaycastNonAlloc(transform.position, aimDirection, m_raycastHits); i++)
            {
                var hit = m_raycastHits[i];

                // Ignore triggers
                if (hit.collider.isTrigger)
                    continue;

                // Ignore backfacing collisions
                var dot = Vector2.Dot(aimDirection, hit.normal);
                if (dot > 0)
                    continue;

                // Calculate closest distance
                if(hit.distance < distance)
                    distance = hit.distance;
            }

            // Set the end point of the lineRenderer
            var aimPosition = aimDirection * distance;
            m_aimLine.SetPosition(1, aimPosition);
        }


        public override void StartTurn()
        {
            IsTurnActive = true;
            m_currentState = State.Aiming;

            GameValues.Combo = 0;
        }


        private void ShootBall(Vector2 direction)
        {
            // Spawn new ball and setup position and direction
            var ball = Instantiate(m_ballPrefab);
            ball.transform.position = transform.position;
            ball.Setup(direction);
        }


        /// <summary>
        /// Used by <see cref="BallCollectionTrigger"/> to indicate when a ball hit the collection trigger.
        /// Saves the position of the first ball that got catched as the next shooting position.
        /// </summary>
        /// <param name="ball">The ball that got catched.</param>
        public void OnBallCatched(BallController ball)
        {
            // Save the position only for the first catched ball
            if (!m_hasNextPosition)
            {
                m_hasNextPosition = true;

                // Only move along the x-axis
                m_nextPosition = transform.position;
                m_nextPosition.x = ball.transform.position.x;
            }
        }

        /// <summary>
        /// Used by <see cref="BallCollectionTrigger"/> to indicate when a catched ball reached the player.
        /// Destroyes the collected ball.
        /// </summary>
        /// <param name="ball">The ball that got collected.</param>
        public void OnBallCollected(BallController ball)
        {
            Destroy(ball.gameObject);
            m_collectionCounter++;
        }
    }
}
