using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BoardController : TurnHandler
{
    [System.Serializable]
    struct BrickData
    {
        public BrickController Prefab;
        public float Weight;
    }


    [SerializeField] float m_brickProbability = 0.5f;
    [SerializeField] List<BrickData> m_bricks;

    [SerializeField] BoardObject m_powerupPrefab;

    [SerializeField] int m_rows = 3;
    [SerializeField] int m_cols = 3;


    bool m_waitingForObjects;


    public List<BoardObject> Board = new List<BoardObject>();

    public int CurrentLevel { get; private set; }

    public System.Action<BoardObject> OnBoardObjectDestroyed;


    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        var worldPosition = transform.position;
        worldPosition += new Vector3(
            gridPosition.x - m_cols * 0.5f + 0.5f,
            m_rows * 0.5f - gridPosition.y - 0.5f
        );

        return worldPosition;
    }


    private void SpawnNewRow()
    {
        if (Board.Any(obj => obj.GridPosition.y == 0))
            return;

        CurrentLevel++;

        var powerupCol = Random.Range(0, m_cols);
        var totalBrickWeight = m_bricks.Sum(brickData => brickData.Weight);

        for (int i = 0; i < m_cols; i++)
        {
            var position = new Vector2Int(i, 0);

            if (i == powerupCol)
            {
                SpawnObject(m_powerupPrefab, position);
            }
            else if (m_brickProbability > 0 && Random.value <= m_brickProbability)
            {
                var weight = Random.Range(0, totalBrickWeight);
                foreach (var brickData in m_bricks)
                {
                    weight -= brickData.Weight;
                    if(weight <= 0)
                    {
                        SpawnObject(brickData.Prefab, position);
                        break;
                    }
                }
            }
        }
    }


    private void Update()
    {
        if(m_waitingForObjects)
        {
            var objBusy = false;
            foreach (var obj in Board)
            {
                if (obj.IsBusy)
                    objBusy = true;
            }

            if (!objBusy)
            {
                SpawnNewRow();

                m_waitingForObjects = false;
                OnMoveDone?.Invoke();
            }
        }
    }


    public override void MakeMove()
    {
        //m_moveTask = new TaskCompletionSource<int>();

        m_waitingForObjects = true;

        var orderedBoard = Board.OrderByDescending(obj => obj.GridPosition.y);
        foreach (var obj in orderedBoard)
        {
            if (obj.GridPosition.y >= m_rows - 1)
                Debug.Log($"Game Over");

            obj.DoMove();
        }
    }


    public void SpawnObject(BoardObject prefab, Vector2Int position)
    {
        var obj = Instantiate(prefab, transform);
        Board.Add(obj);

        obj.Setup(this, position);
    }

    public void DestroyObject(BoardObject obj)
    {
        OnBoardObjectDestroyed?.Invoke(obj);

        Board.Remove(obj);
        Destroy(obj.gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        var col = Color.green;
        Gizmos.color = col;

        for (int x = 0; x < m_cols; x++)
        {
            for (int y = 0; y < m_rows; y++)
            {
                var pos = GridToWorldPosition(new Vector2Int(x, y));
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }

        col.a = 0.3f;
        Gizmos.color = col;

        for (int x = 0; x < m_cols; x++)
        {
            for (int y = 0; y < m_rows; y++)
            {
                var pos = GridToWorldPosition(new Vector2Int(x, y));
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }
}
