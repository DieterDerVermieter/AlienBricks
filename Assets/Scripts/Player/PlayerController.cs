using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DieterDerVermieter
{
    public class PlayerController : TurnHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Transform m_ship;

        [Header("Aim Indication")]
        [SerializeField] private LineRenderer m_aimLine;

        [Header("Shooting")]
        [SerializeField] private Transform m_ballContainer;
        [SerializeField] private BallController m_ballPrefab;

        [SerializeField] private float m_maxAimAngle = 70.0f;
        [SerializeField] private float m_shootDelay = 0.1f;

        [SerializeField] private AudioClip m_shootSound;

        [SerializeField] private float m_minBallSpeed = 10.0f;
        [SerializeField] private float m_maxBallSpeed = 20.0f;

        [Header("Ball Selection")]
        [SerializeField] private Transform m_ballSelectionItemContainer;
        [SerializeField] private BallSelectionItem m_ballSelectionItemPrefab;

        [SerializeField] private BallData[] m_balls;

        [Header("Force Field")]
        [SerializeField] private Transform m_forceFieldContainer;
        [SerializeField] private GameObject m_forceFieldPrefab;

        [SerializeField] private float m_forceFieldCooldown = 5.0f;
        [SerializeField] private Image m_forceFieldCooldownImage;


        private Camera m_mainCamera;
        private RaycastHit2D[] m_raycastHits = new RaycastHit2D[10];

        private enum State
        {
            Aiming,
            Shooting,
            Collecting
        }

        private State m_currentState;

        private bool m_isPointerInsideArena;

        private bool m_isAiming;
        private Vector2 m_aimPosition;
        private Vector2 m_aimDirection = Vector2.up;

        private BallData m_shootingBall;
        private Vector2 m_shootingDirection;
        private int m_shootingCount;

        private int m_shootingCounter;
        private float m_shootingTimer;

        private int m_collectionCounter;
        private float m_collectionTimer;

        private bool m_hasNextPosition;
        private Vector3 m_nextPosition;

        private BallSelectionItem[] m_ballSelectionItems;
        private BallSelectionItem m_selectedBallSelectionItem;

        private float m_forceFieldTimer;


        public Transform Ship => m_ship;


        private void Awake()
        {
            m_mainCamera = Camera.main;
        }


        private void Start()
        {
            m_ballSelectionItems = new BallSelectionItem[m_balls.Length];
            for (int i = 0; i < m_balls.Length; i++)
            {
                var ballData = m_balls[i];

                var ballSelectionItem = Instantiate(m_ballSelectionItemPrefab, m_ballSelectionItemContainer);

                ballSelectionItem.Setup(this, ballData);
                ballSelectionItem.SetCooldown(0);
                ballSelectionItem.SetSelected(false);

                m_ballSelectionItems[i] = ballSelectionItem;
            }
        }


        private void Update()
        {
            // Only enable the aim line if we are currently aiming
            // Also disable aim line when we are not inside the arena
            m_aimLine.gameObject.SetActive(IsTurnActive && m_currentState == State.Aiming && m_isAiming && m_isPointerInsideArena);

            if (!IsTurnActive)
                return;

            // Countdown forceField cooldown
            if (m_forceFieldTimer > 0)
                m_forceFieldTimer -= Time.deltaTime;

            // Update forceField cooldown indicator
            m_forceFieldCooldownImage.fillAmount = m_forceFieldTimer / m_forceFieldCooldown;

            switch (m_currentState)
            {
                case State.Aiming:
                    {
                        UpdateAimIndicator(m_aimDirection);
                    }
                    break;

                case State.Shooting:
                    {
                        m_shootingTimer -= Time.deltaTime;
                        if (m_shootingTimer <= 0)
                        {
                            m_shootingTimer += m_shootDelay;
                            m_shootingCounter++;

                            ShootBall();
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
                        m_collectionTimer += Time.deltaTime;

                        var maxSpeedTime = 10.0f;
                        if (m_collectionTimer > maxSpeedTime)
                            GameValues.BallSpeed = maxSpeedTime;

                        if (m_hasNextPosition)
                        {
                            // Smoothly move to next position
                            var positionLerpTime = 10.0f * Time.deltaTime;
                            Ship.transform.position = Vector3.Lerp(Ship.transform.position, m_nextPosition, positionLerpTime);
                        }

                        // Are all balls collected
                        if (m_collectionCounter >= m_shootingCount)
                        {
                            // Snap to next position if it wasn't reached
                            Ship.transform.position = m_nextPosition;

                            // Reset forceField cooldown
                            m_forceFieldTimer = 0;

                            // Destroy all active forceFields
                            for (int i = 0; i < m_forceFieldContainer.childCount; i++)
                            {
                                var ability = m_forceFieldContainer.GetChild(i).gameObject;
                                Destroy(ability);
                            }

                            // End turn
                            IsTurnActive = false;
                        }
                    }
                    break;

                default:
                    break;
            }
        }


        private void OnAim(InputValue value)
        {
            if(!IsTurnActive)
                return;

            Vector2 mousePosition = m_mainCamera.ScreenToWorldPoint(value.Get<Vector2>());
            m_aimPosition = mousePosition;

            if (m_currentState == State.Aiming)
            {
                // Enable aim line
                m_isAiming = true;

                // Caculate aim direction based on mouse position
                Vector2 aimDirection = (mousePosition - (Vector2)Ship.transform.position).normalized;

                // Calculate aim angle
                var aimAngle = Vector2.SignedAngle(Vector2.up, aimDirection);
                var maxRad = m_maxAimAngle * Mathf.Deg2Rad;

                // Constraint aim direction based on a maximum aim angle
                if (aimAngle > m_maxAimAngle) aimDirection = new Vector2(-Mathf.Sin(maxRad), Mathf.Cos(maxRad));
                if (aimAngle < -m_maxAimAngle) aimDirection = new Vector2(-Mathf.Sin(-maxRad), Mathf.Cos(-maxRad));

                m_aimDirection = aimDirection;
            }
        }

        private void OnFire()
        {
            if (!IsTurnActive)
                return;

            // Disable aim line
            m_isAiming = false;

            // Prevent fire when not inside the arena
            if (!m_isPointerInsideArena)
                return;

            if (m_currentState == State.Aiming)
            {
                // Start shooting
                Shoot();
            }
            else
            {
                // Can spawn another forceField?
                if (m_forceFieldTimer <= 0)
                {
                    m_forceFieldTimer = m_forceFieldCooldown;

                    // Spawn ability
                    SpawnForceField(m_aimPosition);
                }
            }
        }


        private void Shoot()
        {
            if (m_selectedBallSelectionItem == null)
                return;

            // Start shooting
            m_currentState = State.Shooting;

            // Set relevant values
            m_shootingBall = m_selectedBallSelectionItem.Data;
            m_shootingDirection = m_aimDirection;
            m_shootingCount = GameValues.BallCount;

            m_shootingCounter = 0;
            m_shootingTimer = 0;

            m_collectionCounter = 0;
            m_collectionTimer = 0;

            m_hasNextPosition = false;

            // Reset ball selection
            m_selectedBallSelectionItem.ResetCooldown();
            m_selectedBallSelectionItem.SetSelected(false);
            m_selectedBallSelectionItem = null;
        }


        private void UpdateAimIndicator(Vector2 aimDirection)
        {
            // Calculate the closest distance before a ball would collide with something
            var distance = Mathf.Infinity;
            for (int i = 0; i < Physics2D.RaycastNonAlloc(Ship.transform.position, aimDirection, m_raycastHits); i++)
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

            GameValues.BallSpeed = m_minBallSpeed;
            GameValues.Combo = 0;

            // Countdown all balls and select the first available one
            for (int i = 0; i < m_ballSelectionItems.Length; i++)
            {
                var item = m_ballSelectionItems[i];
                item.CountdownCooldown();

                if (m_selectedBallSelectionItem == null && item.Cooldown <= 0)
                {
                    m_selectedBallSelectionItem = item;
                    item.SetSelected(true);
                }
            }
        }


        private void ShootBall()
        {
            // Spawn new ball and setup position and direction
            var ball = Instantiate(m_ballPrefab, Ship.transform.position, Quaternion.identity, m_ballContainer);
            ball.Setup(m_shootingBall, m_shootingDirection);

            SoundEffectManager.Instance.PlayAudioClip(m_shootSound);
        }


        private void SpawnForceField(Vector2 position)
        {
            // Spawn new forceField and setup position
            var ability = Instantiate(m_forceFieldPrefab, position, Quaternion.identity, m_forceFieldContainer);
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
                m_nextPosition = Ship.transform.position;
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


        public void OnPointerEnter(PointerEventData eventData)
        {
            m_isPointerInsideArena = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_isPointerInsideArena = false;
        }


        public void OnBallSelectionItemClicked(BallSelectionItem item)
        {
            if (item == null)
                return;

            if (item.Cooldown > 0)
                return;

            if (m_selectedBallSelectionItem != null)
                m_selectedBallSelectionItem.SetSelected(false);

            m_selectedBallSelectionItem = item;
            item.SetSelected(true);
        }
    }
}
