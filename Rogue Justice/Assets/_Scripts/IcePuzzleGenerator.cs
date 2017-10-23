using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class IcePuzzleGenerator : MonoBehaviour {

    private const int WIDTH = 8;
    private const int HEIGHT = 11;

    private Dictionary<string, GridState> existingGridStates = new Dictionary<string, GridState>();

    private Grid grid;

    private int duration = 0;

    private void Start() {
        for (int i = 0; i < 10000; i++) {
            GenerateIcePuzzle();
            int count = SolveGrid();
            if (count > 10) {
                Debug.Log("SURE");
            }
        }
    }

    private void GenerateIcePuzzle() {
        grid = new Grid();

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

        sb[Random.Range(0, 0)] = '!';
        grid.data = sb.ToString();
        grid.goal = Random.Range(1, 88);
        grid.player = 0;
    }

    private int SolveGrid() {
        Queue<GridState> queue = new Queue<GridState>();
        queue.Enqueue(new GridState(grid.data, 0));
        GridState gridState;

        while (queue.Count > 0) {
            gridState = queue.Dequeue();

            if (gridState.data[grid.goal] == '!') {
                return gridState.distance;
            }

            queue = ExploreGrid(gridState, queue);
        }

        return -1;
    }

    private bool IsValidMove(int player, int move) {
        return (player + move >= 0 && player + move < 88);
    }

    private Queue<GridState> ExploreGrid(GridState gridState, Queue<GridState> queue) {
        char[] dest = new char[gridState.data.Length];
        gridState.data.CopyTo(0, dest, 0, dest.Length);

        int movement = 1;
        if (IsValidMove(grid.player, -WIDTH * movement)) {
            while (gridState.data[grid.player - WIDTH * movement] == '#') {
                movement++;
                if ((IsValidMove(grid.player, -WIDTH * movement) && gridState.data[grid.player - WIDTH * movement] == 'O') || !IsValidMove(grid.player, -WIDTH * movement)) {
                    grid.player = grid.player - WIDTH * (movement - 1);
                    dest[grid.player] = '!';
                    string downGrid = new string(dest);

                    if (!existingGridStates.ContainsKey(downGrid)) {
                        GridState newGridState = new GridState(downGrid, gridState.distance + 1);
                        existingGridStates.Add(downGrid, newGridState);
                        queue.Enqueue(newGridState);
                    }

                    break;
                }
            }
        }

        movement = 1;
        if (IsValidMove(grid.player, 1 * movement)) {
            while (gridState.data[grid.player + 1 * movement] == '#') {
                movement++;
                if ((IsValidMove(grid.player, 1 * movement) && gridState.data[grid.player + 1 * movement] == 'O') || (grid.player + 1 * movement) % 8 == 0) {
                    grid.player = grid.player + 1 * (movement - 1);
                    dest[grid.player] = '!';
                    string rightGrid = new string(dest);

                    if (!existingGridStates.ContainsKey(rightGrid)) {
                        GridState newGridState = new GridState(rightGrid, gridState.distance + 1);
                        existingGridStates.Add(rightGrid, newGridState);
                        queue.Enqueue(newGridState);
                    }

                    break;
                }
            }
        }

        movement = 1;

        if (IsValidMove(grid.player, WIDTH * movement)) {
            while (gridState.data[grid.player + WIDTH * movement] == '#') {
                movement++;
                if ((IsValidMove(grid.player, WIDTH * movement) && gridState.data[grid.player + WIDTH * movement] == 'O') || !IsValidMove(grid.player, WIDTH * movement)) {
                    grid.player = grid.player + WIDTH * (movement - 1);
                    dest[grid.player] = '!';
                    string upGrid = new string(dest);

                    if (!existingGridStates.ContainsKey(upGrid)) {
                        GridState newGridState = new GridState(upGrid, gridState.distance + 1);
                        existingGridStates.Add(upGrid, newGridState);
                        queue.Enqueue(newGridState);
                    }

                    break;
                }
            }
        }

        movement = 1;
        if (IsValidMove(grid.player, -1 * movement)) {
            while (gridState.data[grid.player - 1 * movement] == '#') {
                movement++;
                if ((IsValidMove(grid.player, -1 * movement) && gridState.data[grid.player - 1 * movement] == 'O') || (grid.player - 1 * movement) % 7 == 0) {
                    grid.player = grid.player - 1 * (movement - 1);
                    dest[grid.player] = '!';
                    string leftGrid = new string(dest);

                    if (!existingGridStates.ContainsKey(leftGrid)) {
                        GridState newGridState = new GridState(leftGrid, gridState.distance + 1);
                        existingGridStates.Add(leftGrid, newGridState);
                        queue.Enqueue(newGridState);
                    }

                    break;
                }
            }
        }

        return queue;
    }

    private struct Grid {
        public string data;
        public int goal;
        public int player;

        public Grid(string data, int goal, int player) {
            this.data = data;
            this.goal = goal;
            this.player = player;
        }
    }

    private struct GridState {
        public string data;
        public int distance;

        public GridState(string grid, int distance) {
            this.data = grid;
            this.distance = distance;
        }
    }
}
