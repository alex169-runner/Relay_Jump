using System;
using UnityEngine;

internal class Map
{
    public static int size = 20;
    public static float unit = 1.0f;

    private bool[,] occupied = new bool[size, size];

    public Map()
    {
        for (int i = 0; i < size; ++i) {
            for (int j = 0; j < size; ++j) {
                occupied[i, j] = false;
            }
        }
    }

    public Map(Map other)
    {
        Array.Copy(other.occupied, occupied, other.occupied.Length);
    }
    
    public static Vector2Int GetRandomPos()
    {
        int x = UnityEngine.Random.Range(0, size);
        int y = UnityEngine.Random.Range(0, size);
        return new Vector2Int(x, y);
    }

    public static Vector2Int Reflection(Vector2Int obj, Vector2Int centre)
    {
        int X = centre.x * 2 - obj.x, Y = centre.y * 2 - obj.y;

        if (X >= size) X = size - (X - size + 1);
        else if (X < 0) X = -X - 1;
        if (Y >= size) Y = size - (Y - size + 1);
        else if (Y < 0) Y = -Y - 1;

        return new(X, Y);
    }

    public static Vector3 GetRealPosition(Vector2Int pos)
    {
        Vector3 realPos = new()
        {
            x = pos.x * unit + unit / 2 - (size / 2.0f) * unit,
            y = pos.y * unit + unit / 2 - (size / 2.0f) * unit
        };

        return realPos;
    }

    public static bool IsInside(Vector2Int pos)
    {
        return pos.x < size && pos.y < size && pos.x >= 0 && pos.y >= 0;
    }

    public bool IsOccupied(Vector2Int pos)
    {
        return occupied[pos.x, pos.y];
    }

    public void GetOccupied(Vector2Int pos)
    {
        occupied[pos.x, pos.y] = true;
    }

    public void GetVacant(Vector2Int pos)
    {
        occupied[pos.x, pos.y] = false;
    }

    public bool IsSame(Map other)
    {
        for (int i = 0; i < size; ++i) {
            for (int j = 0; j < size; ++j) {
                if (occupied[i, j] ^ other.occupied[i, j]) {
                    return false;
                }
            }
        }
        return true;
    }
}
