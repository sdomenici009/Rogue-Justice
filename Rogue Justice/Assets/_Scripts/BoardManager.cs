using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {

    private static BoardManager _instance;
    public static BoardManager Instance {
        get {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<BoardManager>();
            return _instance;
        }
    }

    [SerializeField]
    private int width, height;

    [SerializeField]
    private GameObject boardTilePrefab;

    [SerializeField]
    private Transform boardTileParent;

    private BoardTile[,] boardTiles;
    private Transform[,] boardTileTransforms;

    private Dictionary<BoardTile, BoardTile> cameFrom = new Dictionary<BoardTile, BoardTile>();
    private Dictionary<BoardTile, float> costSoFar = new Dictionary<BoardTile, float>();

    [SerializeField]
    private Sprite tempWallSprite;

    public Vector3 GetBoardTileTransformPosition(int x, int y) {
        return boardTileTransforms[x, y].position;
    }
    
    public void Initialize() {
        InitializeBoard();
        InitializeBoardTransforms();
    }

    private void InitializeBoard() {
        boardTiles = new BoardTile[width, height];

        for (int j = 0; j < height; j++) {
            for (int i = 0; i < width; i++) {
                bool isWalkable = true;

                if (j == 0 || j == height - 1 || Random.Range(0, 3) == 0) {
                    isWalkable = false;
                }

                boardTiles[i, j] = new BoardTile(i, j, isWalkable);

                if (i > 0) {
                    boardTiles[i, j].Neighbors.Add(boardTiles[i - 1, j]);
                    boardTiles[i - 1, j].Neighbors.Add(boardTiles[i, j]);
                }

                if (j > 0) {
                    boardTiles[i, j].Neighbors.Add(boardTiles[i, j - 1]);
                    boardTiles[i, j - 1].Neighbors.Add(boardTiles[i, j]);
                }
            }
        }

        Player.Instance.currentTile = boardTiles[0, 1];
    }

    private void InitializeBoardTransforms() {
        boardTileTransforms = new Transform[width, height];

        for (int j = 0; j < height; j++) {
            for (int i = 0; i < width; i++) {
                boardTileTransforms[i, j] = ((GameObject)Instantiate(boardTilePrefab, boardTileParent)).transform;

                if (!boardTiles[i, j].IsWalkable) {
                    boardTileTransforms[i, j].GetComponent<Image>().sprite = tempWallSprite;
                }

                BoardTile goal = boardTiles[i, j];
                boardTileTransforms[i, j].GetComponent<Button>().onClick.AddListener(() => goal.PathTo());
            }
        }
    }

    static public float Heuristic(BoardTile a, BoardTile b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public List<BoardTile> GetPath(BoardTile initialTile, BoardTile targetTile) {
        cameFrom.Clear();
        costSoFar.Clear();

        cameFrom[initialTile] = initialTile;
        costSoFar[initialTile] = 0;

        PriorityQueue<BoardTile> frontier = new PriorityQueue<BoardTile>();
        frontier.Enqueue(initialTile, 0);

        while (frontier.Count() > 0) {
            BoardTile currentTile = frontier.Dequeue();

            if (currentTile.x == targetTile.x && currentTile.y == targetTile.y) {
                return RetracePath(initialTile, targetTile);
            }

            for (int i = 0; i < currentTile.Neighbors.Count; i++) {
                float newCost = costSoFar[currentTile] + 1;

                BoardTile next = currentTile.Neighbors[i];

                if (next.IsWalkable && (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])) {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(next, targetTile);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = currentTile;
                }
            }
        }

        return null;
    }

    private List<BoardTile> RetracePath(BoardTile initialTile, BoardTile targetTile) {
        BoardTile currentTile = targetTile;
        List<BoardTile> path = new List<BoardTile>();

        while (true) {
            if(currentTile.x == initialTile.x && currentTile.y == initialTile.y) {
                break;
            }

            path.Insert(0, currentTile);
            currentTile = cameFrom[currentTile];
        }

        path.Insert(0, currentTile);

        return path;
    }
}
