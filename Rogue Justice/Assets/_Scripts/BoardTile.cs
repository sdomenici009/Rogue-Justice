using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : IComparable<BoardTile>{
    public float priority = -1;
    public int x;
    public int y;
    public bool IsWalkable;
    public List<BoardTile> neighbors;

    public BoardTile(int x, int y, bool isWalkable) {
        this.x = x;
        this.y = y;
        this.IsWalkable = isWalkable;
        this.neighbors = new List<BoardTile>();
    }

    public int CompareTo(BoardTile other) {
        if (this.priority < other.priority) return -1;
        else if (this.priority > other.priority) return 1;
        else return 0;
    }
}
