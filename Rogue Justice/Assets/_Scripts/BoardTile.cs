using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile {
    public int x;
    public int y;
    public bool IsWalkable;
    public List<BoardTile> Neighbors;

    public BoardTile(int x, int y, bool isWalkable) {
        this.x = x;
        this.y = y;
        this.IsWalkable = isWalkable;
        this.Neighbors = new List<BoardTile>();
    }

    public void PathTo() {
        Player.Instance.FindPathTo(this);
    }
}
