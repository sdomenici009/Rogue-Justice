using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardTile {
    public int x;
    public int y;
    public bool IsWalkable;

    public BoardTile(int x, int y, bool isWalkable) {
        this.x = x;
        this.y = y;
        this.IsWalkable = isWalkable;
    }
}
