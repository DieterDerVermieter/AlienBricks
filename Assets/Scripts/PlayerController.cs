using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class PlayerController : TurnHandler
{
    [SerializeField] DeathZone m_deathZone;
    [SerializeField] BoardController m_boardController;

    [SerializeField] LineRenderer m_aimLine;
    [SerializeField] Transform m_aimHead;

    [SerializeField] BallController m_ballPrefab;

    [SerializeField] float m_maxShootAngle = 70.0f;

    [SerializeField] float m_shootDelay = 0.1f;

    [SerializeField] float m_ballRadius = 0.2f;
    [SerializeField] float m_ballSpeed = 5.0f;

    [SerializeField] TMP_Text m_comboText;
    [SerializeField] TMP_Text m_roundScoreText;
    [SerializeField] TMP_Text m_scoreText;


    RaycastHit2D[] m_raycastHits = new RaycastHit2D[10];

    Camera m_mainCamera;

    bool m_canShoot;

    bool m_waitingForBalls;
    int m_ballsMissing;

    TaskCompletionSource<int> m_moveTask;

    bool m_waitingForPosition;
    Vector3 m_nextPosition;


    public int Combo { get; private set; }
    public int RoundScore { get; private set; }
    public int Score { get; private set; }


    public static int BallCount;


    private void Awake()
    {
        m_mainCamera = Camera.main;
    }


    private void OnEnable()
    {
        m_deathZone.OnBallKilledCallback += OnBallKilled;
        m_deathZone.OnBallCollectedCallback += OnBallCollected;

        m_boardController.OnBoardObjectDestroyed += OnBoardObjectDestroyed;
    }

    private void OnDisable()
    {
        m_deathZone.OnBallKilledCallback -= OnBallKilled;
        m_deathZone.OnBallCollectedCallback -= OnBallCollected;

        m_boardController.OnBoardObjectDestroyed -= OnBoardObjectDestroyed;
    }


    private void Start()
    {
        BallCount = 1;
        m_nextPosition = transform.position;
    }


    private void Update()
    {
        m_aimHead.gameObject.SetActive(m_canShoot);
        m_aimLine.gameObject.SetActive(m_canShoot);

        if (m_canShoot)
        {
            var mousePosition = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            var inputDirection = (mousePosition - transform.position).normalized;

            var angle = Vector2.SignedAngle(Vector2.up, inputDirection);
            var maxRad = m_maxShootAngle * Mathf.Deg2Rad;

            if (angle > m_maxShootAngle) inputDirection = new Vector2(-Mathf.Sin(maxRad), Mathf.Cos(maxRad));
            if (angle < -m_maxShootAngle) inputDirection = new Vector2(-Mathf.Sin(-maxRad), Mathf.Cos(-maxRad));

            var distance = Mathf.Infinity;
            for (int i = 0; i < Physics2D.CircleCastNonAlloc(transform.position, m_ballRadius, inputDirection, m_raycastHits); i++)
            {
                var hit = m_raycastHits[i];

                var dot = Vector2.Dot(inputDirection, hit.normal);
                if (dot > 0)
                    continue;

                if (!hit.collider.isTrigger && hit.distance < distance)
                {
                    distance = hit.distance;
                }
            }

            var aimPosition = inputDirection * distance;

            m_aimHead.localPosition = aimPosition;
            m_aimHead.localScale = Vector3.one * m_ballRadius * 2.0f;
            m_aimLine.SetPosition(1, aimPosition - inputDirection * m_ballRadius);

            if (Input.GetButtonDown("Fire1"))
            {
                m_canShoot = false;
                m_ballsMissing = BallCount;
                m_waitingForPosition = true;

                StartCoroutine(ShootBalls(inputDirection, m_ballPrefab, BallCount, m_shootDelay));
            }
        }

        if(m_waitingForBalls)
        {
            transform.position = Vector3.Lerp(transform.position, m_nextPosition, 10.0f * Time.deltaTime);
        }
    }


    private IEnumerator ShootBalls(Vector2 direction, BallController ballPrefab, int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            ShootBall(direction, ballPrefab);
            yield return new WaitForSeconds(delay);
        }

        m_waitingForBalls = true;
    }

    private void ShootBall(Vector2 direction, BallController ballPrefab)
    {
        var ball = Instantiate(ballPrefab);
        ball.transform.position = transform.position;
        ball.Setup(this, direction, m_ballRadius, m_ballSpeed);
    }


    private void OnBallKilled(Vector3 position)
    {
        if (!m_waitingForPosition)
            return;

        m_waitingForPosition = false;
        m_nextPosition.x = position.x;
    }

    private void OnBallCollected()
    {
        m_ballsMissing--;
        if(m_ballsMissing <= 0)
        {
            Score += RoundScore;

            m_comboText.text = "";
            m_roundScoreText.text = "";
            m_scoreText.text = $"{Score}";

            OnMoveDone?.Invoke();
        }
    }


    private void OnBoardObjectDestroyed(BoardObject obj)
    {
        var brick = obj as BrickController;
        if (brick == null)
            return;

        Combo++;
        RoundScore += brick.PointsWorth * Combo;

        m_comboText.text = $"{Combo}x";
        m_roundScoreText.text = $"{RoundScore}";
    }


    public override void MakeMove()
    {
        m_canShoot = true;
        m_waitingForBalls = false;

        Combo = 0;
        RoundScore = 0;
    }
}
