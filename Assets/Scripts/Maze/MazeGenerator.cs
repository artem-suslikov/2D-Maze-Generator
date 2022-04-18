using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public struct Cell
    {
        public int X { get; }
        public int Y { get; }

        public Cell(int xCoord, int yCoord)
        {
            X = xCoord;
            Y = yCoord;
        }
    }
    public class MazeGenerator
    {
        enum State { unvisited, blocked, visited };

        private readonly int width = 6;
        private readonly int height = 6;
        private readonly int entranceX = 1;
        private readonly int entranceY = 1;
        private readonly int exitX = 4;
        private readonly int exitY = 4;
        private readonly bool[,] maze;
        private readonly bool[,] path;

        private readonly State[,] field;
        private readonly Stack<Cell> current = new Stack<Cell>();
        private readonly bool[,] forks;
        private int forksCounter = 0;

        public MazeGenerator(int w, int h, int entX, int entY, int extX, int extY)
        {
            if (w > width && w < 1000) { width = w; }
            if (h > height && h < 1000) { height = h; }
            if (entX > 0 && entX < width - 1) { entranceX = entX; }
            if (entY > 0 && entY < height - 1) { entranceY = entY; }
            if (extX > 0 && extX < width - 1) { exitX = extX; }
            if (extY > 0 && extY < height - 1) { exitY = extY; }
            field = new State[width, height];
            forks = new bool[width, height];
            maze = new bool[width, height];
            path = new bool[width, height];
            GenerateMaze();
        }
        
        // MAIN SCRIPT of the maze generation
        public bool[,] GenerateMaze()
        {
            // making borders
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || i == (width - 1) || j == 0 || j == (height - 1))
                    {
                        field[i, j] = State.blocked;
                    }
                }
            }

            // placing start cell
            current.Push(new Cell(entranceX, entranceY));
            field[current.Peek().X, current.Peek().Y] = State.visited;

            do
            {
                // list of nearby cell free to step
                List<Cell> freeCells = new List<Cell>(CheckFourDirections());

                // checking each of free cells if exists
                if (freeCells.Count > 0)
                {
                    List<Cell> aroundCells = new List<Cell>(freeCells);
                    if (!CheckForFinish(aroundCells))
                    {
                        foreach (Cell c in aroundCells)
                        {
                            if (CheckVisitedAround(c.X, c.Y))
                            {
                                freeCells.Remove(c);
                            }
                        }

                        // if at least one free cell exists
                        if (freeCells.Count > 0)
                        {
                            // creating fork if needed
                            if (freeCells.Count > 1)
                            {
                                AddFork();
                            }
                            // making next step
                            MakeStep(freeCells);
                        }
                        // if no free cells exist
                        else
                        {
                            RemoveFork();
                            MoveBackToFork();
                        }
                    }
                    // if finish was founded
                    else
                    {
                        MakePath();
                        MoveBackToFork();
                    }
                }
                // if no free cells exist
                else
                {
                    RemoveFork();
                    MoveBackToFork();
                }
            }
            while (forksCounter > 0);

            // filling the array of the generated maze
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (field[i, j] == State.visited)
                    {
                        maze[i, j] = true;
                    }
                }
            }

            return maze;
        }

        // returns a list of unvisited cells nearby (N-E-S-W)
        private List<Cell> CheckFourDirections()
        {
            List<Cell> available = new List<Cell>();
            int xCurrent = current.Peek().X;
            int yCurrent = current.Peek().Y;

            if (field[xCurrent, yCurrent - 1] == State.unvisited) available.Add(new Cell(xCurrent, yCurrent - 1));
            if (field[xCurrent + 1, yCurrent] == State.unvisited) available.Add(new Cell(xCurrent + 1, yCurrent));
            if (field[xCurrent, yCurrent + 1] == State.unvisited) available.Add(new Cell(xCurrent, yCurrent + 1));
            if (field[xCurrent - 1, yCurrent] == State.unvisited) available.Add(new Cell(xCurrent - 1, yCurrent));

            return available;
        }

        // returns true if there is a finisch cell in the list
        private bool CheckForFinish(List<Cell> cells)
        {
            bool result = false;

            foreach (Cell c in cells)
            {
                if (c.X == exitX && c.Y == exitY)
                {
                    result = true;
                    field[c.X, c.Y] = State.visited;
                }
            }
            return result;
        }

        // returns true if cell borders with visited place
        private bool CheckVisitedAround(int x, int y)
        {
            bool result = false;
            int currentX = current.Peek().X;
            int currentY = current.Peek().Y;

            // veritcal
            if (x == currentX)
            {
                //up
                if (y > currentY)
                {
                    if (field[x - 1, y + 0] == State.visited) result = true;
                    if (field[x - 1, y + 1] == State.visited) result = true;
                    if (field[x + 0, y + 1] == State.visited) result = true;
                    if (field[x + 1, y + 1] == State.visited) result = true;
                    if (field[x + 1, y + 0] == State.visited) result = true;
                }
                //down
                else
                {
                    if (field[x + 1, y + 0] == State.visited) result = true;
                    if (field[x + 1, y - 1] == State.visited) result = true;
                    if (field[x + 0, y - 1] == State.visited) result = true;
                    if (field[x - 1, y - 1] == State.visited) result = true;
                    if (field[x - 1, y + 0] == State.visited) result = true;
                }
            }
            // horizontal
            else
            {
                //left
                if (x < currentX)
                {
                    if (field[x + 0, y - 1] == State.visited) result = true;
                    if (field[x - 1, y - 1] == State.visited) result = true;
                    if (field[x - 1, y + 0] == State.visited) result = true;
                    if (field[x - 1, y + 1] == State.visited) result = true;
                    if (field[x + 0, y + 1] == State.visited) result = true;
                }
                //right
                else
                {
                    if (field[x + 0, y + 1] == State.visited) result = true;
                    if (field[x + 1, y + 1] == State.visited) result = true;
                    if (field[x + 1, y + 0] == State.visited) result = true;
                    if (field[x + 1, y - 1] == State.visited) result = true;
                    if (field[x + 0, y - 1] == State.visited) result = true;
                }
            }
            return result;
        }

        // making step randomly
        private void MakeStep(List<Cell> availableCells)
        {
            // making next move randomly
            current.Push(availableCells[Random.Range(0, availableCells.Count)]);

            // marks current cell as visited if needed
            if (field[current.Peek().X, current.Peek().Y] != State.visited)
            {
                field[current.Peek().X, current.Peek().Y] = State.visited;
            }
        }

        // creates a fork in current cell
        private void AddFork()
        {
            if (!forks[current.Peek().X, current.Peek().Y])
            {
                forks[current.Peek().X, current.Peek().Y] = true;
                forksCounter++;
            }
        }

        // removes a fork from current cell if exists
        private void RemoveFork()
        {
            if (forks[current.Peek().X, current.Peek().Y])
            {
                forks[current.Peek().X, current.Peek().Y] = false;
                forksCounter--;
            }
        }

        // clears current stack to the last fork
        private void MoveBackToFork()
        {
            if (forksCounter > 0)
            {
                while (!forks[current.Peek().X, current.Peek().Y])
                {
                    current.Pop();
                }
            }
        }

        // creates the solution sequence
        private void MakePath()
        {
            Stack<Cell> pathTemp = new Stack<Cell>(current);
            while(pathTemp.Count > 0)
            {
                Cell tempCell = pathTemp.Pop();
                path[tempCell.X, tempCell.Y] = true;
            }
            path[exitX, exitY] = true;
        }
        
        // returns the path array
        public bool[,] GetPath()
        {
            return path;
        }
    }
}

