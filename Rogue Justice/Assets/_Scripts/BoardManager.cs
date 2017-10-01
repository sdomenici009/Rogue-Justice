using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    [SerializeField]
    private int width, height;

    private BoardTile[,] boardTiles;

    void Start() {
        InitializeBoard();
    }

    private void InitializeBoard() { 
        boardTiles = new BoardTile[width, height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                bool isWalkable = true;
                if (j != 0 && j != height - 1)
                    isWalkable = false;

                boardTiles[i, j] = new BoardTile(i, j, isWalkable);
            }
        }
    }

    void Update() {

    }
}
