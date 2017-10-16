using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private static Player _instance;
    public static Player Instance {
        get {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<Player>();
            return _instance;
        }
    }

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private BoardManager boardManager;

    public BoardTile currentTile;

    public void FindPathTo(BoardTile targetTile) {
        List<BoardTile> path = boardManager.GetPath(currentTile, targetTile);
        
        if(path != null) {
            currentTile = targetTile;
            StartCoroutine(AnimatePath(path));
        }
    }

    private IEnumerator AnimatePath(List<BoardTile> path) {
        for (int i = 0; i < path.Count; i++) {
            yield return StartCoroutine(LerpPosition(playerTransform, BoardManager.Instance.GetBoardTileTransformPosition(path[i].x, path[i].y), .05f));
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
