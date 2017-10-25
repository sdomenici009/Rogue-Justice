using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class IcePuzzleGenerator : MonoBehaviour {

    private const int WIDTH = 8;
    private const int HEIGHT = 11;

    private Dictionary<string, GridState> existingGridStates = new Dictionary<string, GridState>();

    //private Grid grid;

    private int duration = 0;

    [SerializeField]
    private BoardManager boardManager;

    private void Start() {
        for (int i = 0; i < 10000; i++) {
            Grid grid = GenerateIcePuzzle();
            GridState solvedState = SolveGrid(grid);
            if (solvedState != null && solvedState.distance > 10) {
                StringBuilder sb = new StringBuilder(grid.data);
                grid.data = sb.ToString();
                boardManager.LoadGrid(grid);

                /*
                PrintGridFromString(solvedState.data);
                Debug.Log(solvedState.dir);
                while (solvedState.parent != null) {
                    PrintGridFromString(solvedState.parent.data);
                    Debug.Log(solvedState.dir);
                    solvedState = solvedState.parent;
                }*/

                //Debug.Log(solvedState.distance);
                break;
            }
        }

        Debug.Log("DONE");
    }

    private void PrintGridFromString(string s) {
        char[] schar;
        string super = "";
        for (int j = HEIGHT - 1; j >= 0; j--) {
            schar = new char[WIDTH];
            for (int i = 0; i < WIDTH; i++) {
                schar[i] = s[i + j * WIDTH];
            }
            super += new string(schar) + "\n";
        }

        Debug.Log(super);
    }

    private Grid GenerateIcePuzzle() {
        Grid grid = new Grid();

        StringBuilder sb = new StringBuilder(grid.data);

        for (int i = 0; i < WIDTH; i++) {
            for (int j = 0; j < HEIGHT; j++) {
                if (Random.Range(0, 5) == 0) {
                    sb.Append("O");
                } else {
                    sb.Append("#");
                }
            }
        }

        grid.player = Random.Range(0, 88);
        sb[grid.player] = '!';
        grid.data = sb.ToString();
        int rand = Random.Range(0, 88);
        while (rand == grid.player) {
            rand = Random.Range(0, 88);
        }
        grid.goal = rand;

        return grid;
    }

    private GridState SolveGrid(Grid grid) {
        Queue<GridState> queue = new Queue<GridState>();
        char[] dest = new char[grid.data.Length];
        grid.data.CopyTo(0, dest, 0, dest.Length);

        queue.Enqueue(new GridState(new string(dest), grid.player, 0, null, "root"));
        GridState gridState;

        while (queue.Count > 0) {
            gridState = queue.Dequeue();

            if (gridState.data[grid.goal] == '!') {
                return gridState;
            }

            queue = ExploreGrid(gridState, queue);
        }

        return null;
    }

    private bool IsValidMove(int player, int move) {
        return (player + move >= 0 && player + move < 88);
    }

    private Queue<GridState> ExploreGrid(GridState gridState, Queue<GridState> queue) {
        char[] dest = new char[gridState.data.Length];
        gridState.data.CopyTo(0, dest, 0, dest.Length);

        int moveCount = 1;
        if (IsValidMove(gridState.player, -WIDTH * moveCount) && gridState.data[gridState.player - WIDTH * moveCount] == '#') {
            while (true) {
                moveCount++;
                if (!IsValidMove(gridState.player, -WIDTH * moveCount) || gridState.data[gridState.player - WIDTH * moveCount] != '#') {
                    moveCount--;
                    break;
                }
            }

            dest[gridState.player] = '#';
            dest[gridState.player - WIDTH * moveCount] = '!';
            string downGrid = new string(dest);

            if (!existingGridStates.ContainsKey(downGrid)) {
                GridState newGridState = new GridState(downGrid, gridState.player - WIDTH * moveCount, gridState.distance + 1, gridState, "down");
                existingGridStates.Add(downGrid, newGridState);
                queue.Enqueue(newGridState);
            }
        }

        dest = new char[gridState.data.Length];
        gridState.data.CopyTo(0, dest, 0, dest.Length);

        moveCount = 1;
        if (IsValidMove(gridState.player, 1 * moveCount) && gridState.data[gridState.player + 1 * moveCount] == '#' && (gridState.player + 1 * moveCount) % 8 != 0) {
            while (true) {
                moveCount++;
                if (!IsValidMove(gridState.player, 1 * moveCount) || gridState.data[gridState.player + 1 * moveCount] != '#' || (gridState.player + 1 * moveCount) % 8 == 0) {
                    moveCount--;
                    break;
                }
            }

            dest[gridState.player] = '#';
            dest[gridState.player + 1 * moveCount] = '!';
            string rightGrid = new string(dest);

            if (!existingGridStates.ContainsKey(rightGrid)) {
                GridState newGridState = new GridState(rightGrid, gridState.player + 1 * moveCount, gridState.distance + 1, gridState, "right");
                existingGridStates.Add(rightGrid, newGridState);
                queue.Enqueue(newGridState);
            }
        }

        dest = new char[gridState.data.Length];
        gridState.data.CopyTo(0, dest, 0, dest.Length);

        moveCount = 1;

        if (IsValidMove(gridState.player, WIDTH * moveCount) && gridState.data[gridState.player + WIDTH * moveCount] == '#') {
            while (true) {
                moveCount++;
                if (!IsValidMove(gridState.player, WIDTH * moveCount) || gridState.data[gridState.player + WIDTH * moveCount] != '#') {
                    moveCount--;
                    break;
                }
            }

            dest[gridState.player] = '#';
            dest[gridState.player + WIDTH * moveCount] = '!';
            string upGrid = new string(dest);

            if (!existingGridStates.ContainsKey(upGrid)) {
                GridState newGridState = new GridState(upGrid, gridState.player + WIDTH * moveCount, gridState.distance + 1, gridState, "up");
                existingGridStates.Add(upGrid, newGridState);
                queue.Enqueue(newGridState);
            }
        }

        dest = new char[gridState.data.Length];
        gridState.data.CopyTo(0, dest, 0, dest.Length);

        moveCount = 1;

        if (IsValidMove(gridState.player, -1 * moveCount) && gridState.data[gridState.player - 1 * moveCount] == '#' && (gridState.player - 1 * moveCount - 7) % 8 != 0) {
            while (true) {
                moveCount++;
                if (!IsValidMove(gridState.player, -1 * moveCount) || gridState.data[gridState.player - 1 * moveCount] != '#' || (gridState.player - 1 * moveCount - 7) % 8 == 0) {
                    moveCount--;
                    break;
                }
            }

            dest[gridState.player] = '#';
            dest[gridState.player - 1 * moveCount] = '!';
            string leftGrid = new string(dest);

            if (!existingGridStates.ContainsKey(leftGrid)) {
                GridState newGridState = new GridState(leftGrid, gridState.player - 1 * moveCount, gridState.distance + 1, gridState, "left");
                existingGridStates.Add(leftGrid, newGridState);
                queue.Enqueue(newGridState);
            }
        }

        return queue;
    }

    public struct Grid {
        public string data;
        public int goal;
        public int player;

        public Grid(string data, int goal, int player) {
            this.data = data;
            this.goal = goal;
            this.player = player;
        }
    }

    private class GridState {
        public string data;
        public int player;
        public int distance;
        public GridState parent;
        public string dir;

        public GridState(string grid, int player, int distance, GridState parent, string dir) {
            this.data = grid;
            this.player = player;
            this.distance = distance;
            this.parent = parent;
            this.dir = dir;
        }
    }
}
