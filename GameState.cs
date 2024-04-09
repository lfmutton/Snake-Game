using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Game;

public class GameState
{
    public int Rows { get; }
    public int Columns { get; }
    public GridValue[,] Grid { get; }
    public Direction Dir { get; private set; }
    public int Score { get; private set; }
    public bool GameOver { get; private set; }

    private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
    private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
    private readonly Random random = new Random();

    public GameState(int rows, int cols)
    {
        Rows = rows;
        Columns = cols;
        Grid = new GridValue[rows, cols];
        Dir = Direction.Right;

        AddSnake();
        AddFood();
    }

    private void AddSnake()
    {
        int r = Rows / 2;

        for (int c = 1; c <= 3; c++)
        {
            Grid[r, c] = GridValue.Snake;
            snakePositions.AddFirst(new Position(r, c));
        }
    }

    private IEnumerable<Position> EmptyPosition()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (Grid[r, c] == GridValue.Empty)
                    yield return new Position(r, c);
            }
        }
    }
    private void AddFood()
    {
        List<Position> emptyPositions = new List<Position>(EmptyPosition());

        if (emptyPositions.Count == 0)
            return;

        Position pos = emptyPositions[random.Next(emptyPositions.Count)];
        Grid[pos.Row, pos.Column] = GridValue.Food;
    }

    public Position HeadPosition()
    {
        return snakePositions.First.Value;
    }

    public Position TailPosition()
    {
        return snakePositions.Last.Value;
    }

    public IEnumerable<Position> SnakePositions()
    {
        return snakePositions;
    }

    private void AddHead(Position pos)
    {
        snakePositions.AddFirst(pos);
        Grid[pos.Row, pos.Column] = GridValue.Snake;
    }

    private void RemoveTail()
    {
        Position tail = snakePositions.Last.Value;
        Grid[tail.Row, tail.Column] = GridValue.Empty;
        snakePositions.RemoveLast();
    }

    private Direction GetLastDirection()
    {
        if (dirChanges.Count == 0)
        {
            return Dir;
        }
        return dirChanges.Last.Value;
    }

    private bool CanChangeDirection(Direction newDir)
    {
        if (dirChanges.Count == 2)
        {
            return false;
        }

        Direction lastDir = GetLastDirection();
        return newDir != lastDir && newDir != lastDir.Opposite();
    }

    public void ChangeDirection(Direction direction)
    {
        if (CanChangeDirection(direction))
        {
            dirChanges.AddLast(direction);
        }
    }

    private bool OutsideGrid(Position pos)
    {
        return pos.Row < 0 || pos.Row >= Rows || pos.Column < 0 || pos.Column >= Columns;
    }

    private GridValue WillHit(Position newHeadPos)
    {
        if (OutsideGrid(newHeadPos))
        {
            return GridValue.Outside;
        }
        if( newHeadPos == TailPosition())
        {
            return GridValue.Snake;
        }
        return Grid[newHeadPos.Row, newHeadPos.Column];
    }

    public void Move()
    {
        if(dirChanges.Count > 0)
        {
            Dir = dirChanges.First.Value;
            dirChanges.RemoveFirst();
        }

        Position newHeadPod = HeadPosition().Translate(Dir);
        GridValue hit = WillHit(newHeadPod);

        if (hit == GridValue.Snake || hit == GridValue.Outside)
        {
            GameOver = true;
        }
        else if (hit == GridValue.Empty)
        {
            RemoveTail();
            AddHead(newHeadPod);
        }
        else if (hit == GridValue.Food)
        {
            AddHead(newHeadPod);
            Score++;
            AddFood();
        }
    }
} 
