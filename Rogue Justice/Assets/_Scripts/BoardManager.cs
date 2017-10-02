using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {

    [SerializeField]
    private int width, height;

    [SerializeField]
    private GameObject boardTilePrefab;

    [SerializeField]
    private Transform boardTileParent;

    private BoardTile[,] boardTiles;
    private Transform[,] boardTileTransforms;

    #region Prototyping
    [SerializeField]
    private Sprite tempWallSprite;

    [SerializeField]
    private BoardTile tempCurrentBoardTile;

    [SerializeField]
    Transform player;
    #endregion

    void Start() {
        InitializeBoard();
        InitializeBoardTransforms();

        tempCurrentBoardTile = boardTiles[0, 1];
    }

    private void InitializeBoard() {
        boardTiles = new BoardTile[width, height];

        for (int j = 0; j < height; j++) {
            for (int i = 0; i < width; i++) {
                bool isWalkable = true;
                if (j == 0 || j == height - 1 || Random.Range(0, 10) == 0) {
                    isWalkable = false;
                }

                boardTiles[i, j] = new BoardTile(i, j, isWalkable);

                if (i > 0) {
                    boardTiles[i, j].neighbors.Add(boardTiles[i - 1, j]);
                    boardTiles[i - 1, j].neighbors.Add(boardTiles[i, j]);

                    if (j > 0) {
                        boardTiles[i, j].neighbors.Add(boardTiles[i - 1, j - 1]);
                        boardTiles[i - 1, j - 1].neighbors.Add(boardTiles[i, j]);
                    }
                }

                if (j > 0) {
                    boardTiles[i, j].neighbors.Add(boardTiles[i, j - 1]);
                    boardTiles[i, j - 1].neighbors.Add(boardTiles[i, j]);

                    if (i < width - 1) {
                        boardTiles[i, j].neighbors.Add(boardTiles[i + 1, j - 1]);
                        boardTiles[i + 1, j - 1].neighbors.Add(boardTiles[i, j]);
                    }
                }
            }
        }
    }

    private void InitializeBoardTransforms() {
        boardTileTransforms = new Transform[width, height];

        for (int j = 0; j < height; j++) {
            for (int i = 0; i < width; i++) {
                boardTileTransforms[i, j] = ((GameObject)Instantiate(boardTilePrefab, boardTileParent)).transform;
                BoardTile goal = boardTiles[i, j];
                boardTileTransforms[i, j].GetComponent<Button>().onClick.AddListener(() => PathTo(goal));

                if (!boardTiles[i, j].IsWalkable) {
                    boardTileTransforms[i, j].GetComponent<Image>().sprite = tempWallSprite;
                }
            }
        }
    }

    private Dictionary<BoardTile, BoardTile> cameFrom = new Dictionary<BoardTile, BoardTile>();
    private Dictionary<BoardTile, float> costSoFar = new Dictionary<BoardTile, float>();

    static public float Heuristic(BoardTile a, BoardTile b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public void PathTo(BoardTile goal) {
        cameFrom.Clear();
        costSoFar.Clear();

        PriorityQueue<BoardTile> frontier = new PriorityQueue<BoardTile>();
        tempCurrentBoardTile.priority = 0;
        frontier.Enqueue(tempCurrentBoardTile);

        cameFrom[tempCurrentBoardTile] = tempCurrentBoardTile;
        costSoFar[tempCurrentBoardTile] = 0;

        bool foundGoal = false;

        while (frontier.Count() > 0) {
            BoardTile current = frontier.Dequeue();

            if (current.x == goal.x && current.y == goal.y) {
                foundGoal = true;
                break;
            }

            for (int i = 0; i < current.neighbors.Count; i++) {
                float newCost = costSoFar[current] + 1;

                BoardTile next = current.neighbors[i];

                if (next.IsWalkable && (!costSoFar.ContainsKey(next)
                    || newCost < costSoFar[next])) {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(next, goal);
                    next.priority = priority;
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (foundGoal) {
            StartCoroutine(AnimatePath(cameFrom, tempCurrentBoardTile, goal));
            tempCurrentBoardTile = goal;
        }

        //tempCurrentBoardTile = goal;
    }

    private IEnumerator AnimatePath(Dictionary<BoardTile, BoardTile> cameFrom, BoardTile start, BoardTile goal) {
        BoardTile curr = goal;

        List<BoardTile> path = new List<BoardTile>();

        while (true) {
            if (curr.x == start.x && curr.y == start.y) {
                break;
            }

            path.Insert(0, curr);
            curr = cameFrom[curr];
        }

        path.Insert(0, curr);

        Debug.Log(path.Count);

        for (int i = 0; i < path.Count; i++) {
            yield return StartCoroutine(LerpPosition(player, boardTileTransforms[path[i].x, path[i].y].position, .15f));
        }
    }

    private IEnumerator LerpPosition(Transform transform, Vector3 targetPosition, float duration) {
        float progress = 0;
        Vector3 startPosition = transform.position;
        while (progress < 1) {
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            progress += Time.deltaTime / duration;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
