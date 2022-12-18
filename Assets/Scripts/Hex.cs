using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection
{
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
}

public struct Hex
{
    // implementation inspired by https://www.redblobgames.com/grids/hexagons/
    // Using Axial coordinate system for hexagonal tiles with pointy-top orientation.

    private const float Spacing = 1f;

    public readonly int q;
    public readonly int r;
    public int s => -q - r;
    public Hex[] Neighbours => new Hex[]
    {
        this + right,
        this + upRight,
        this + upLeft,
        this + left,
        this + downLeft,
        this + downRight
    };
    public Hex[] DiagonalNeighbours => new Hex[]
    {
        this + diagonalUpRight,
        this + diagonalUp,
        this + diagonalUpLeft,
        this + diagonalDownLeft,
        this + diagonalDown,
        this + diagonalDownRight,
    };
    public int Length => (Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(s)) / 2;

    // use XZ plane in 3D
    public Vector3 WorldPosition => new Vector3(
        Mathf.Sqrt(3) * (q + .5f * r) * Spacing,
        0,
        1.5f * r * Spacing);

    // use XY plane in 2D
    public Vector2 WorldPosition2D => new Vector2(
        Mathf.Sqrt(3) * (q + .5f * r) * Spacing,
        1.5f * r * Spacing);

    public Hex(int q, int r)
    {
        this.q = q;
        this.r = r;
    }
    public Hex[] NeighboursInRange(int range)
    {
        List<Hex> results = new List<Hex>();
        for (int q = -range; q <= range; q++)
            for (int r = Mathf.Max(-range, -range - q); r <= Mathf.Min(range, range - q); r++)
                results.Add(this + new Hex(q, r));

        return results.ToArray();
    }
    public Hex RotateLeft() => new Hex(-s, -q);
    public Hex RotateRight() => new Hex(-r, -s);
    public Hex RotateLeftAround(Hex center) => (this - center).RotateLeft() + center;
    public Hex RotateRightAround(Hex center) => (this - center).RotateRight() + center;

    public static Hex FromWorldPosition(Vector3 worldPosition)
    {
        float q = (Mathf.Sqrt(3) / 3 * worldPosition.x - 1f / 3 * worldPosition.z) / Spacing;
        float r = 2f / 3 * worldPosition.z / Spacing;
        return Round(q, r);
    }
    public static Hex FromWorldPosition(Vector2 worldPosition)
    {
        float q = (Mathf.Sqrt(3) / 3 * worldPosition.x - 1f / 3 * worldPosition.y) / Spacing;
        float r = 2f / 3 * worldPosition.y / Spacing;
        return Round(q, r);
    }
    public static Hex FromDirection(HexDirection direction)
    {
        if (direction == HexDirection.Left) return left;
        if (direction == HexDirection.Right) return right;
        if (direction == HexDirection.UpLeft) return upLeft;
        if (direction == HexDirection.UpRight) return upRight;
        if (direction == HexDirection.DownLeft) return downLeft;
        if (direction == HexDirection.DownRight) return downRight;

        return zero;
    }
    public static int Distance(Hex a, Hex b) => (a - b).Length;
    public static Hex[] Line(Hex startPosition, Hex direction, int length)
    {
        List<Hex> line = new List<Hex>();
        for (int i = 0; i <= length; i++)
            line.Add(startPosition + i * direction);
        return line.ToArray();
    }

    public static readonly Hex zero = new Hex(0, 0);

    public static readonly Hex right = new Hex(1, 0);
    public static readonly Hex upRight = new Hex(0, 1);
    public static readonly Hex upLeft = new Hex(-1, 1);
    public static readonly Hex left = new Hex(-1, 0);
    public static readonly Hex downLeft = new Hex(0, -1);
    public static readonly Hex downRight = new Hex(1, -1);

    public static readonly Hex diagonalUpRight = new Hex(1, 1);
    public static readonly Hex diagonalUp = new Hex(-1, 2);
    public static readonly Hex diagonalUpLeft = new Hex(-2, 1);
    public static readonly Hex diagonalDownLeft = new Hex(-1, -1);
    public static readonly Hex diagonalDown = new Hex(1, -2);
    public static readonly Hex diagonalDownRight = new Hex(2, -1);

    public override bool Equals(object obj)
    {
        if (obj is Hex other)
            return q == other.q && r == other.r;
        return false;
    }
    public override int GetHashCode() => HashCode.Combine(q, r);
    public override string ToString() => $"Hex({q}, {r})";
    public static Hex operator +(Hex a) => a;
    public static Hex operator -(Hex a) => new Hex(-a.q, -a.r);
    public static Hex operator +(Hex a, Hex b) => new Hex(a.q + b.q, a.r + b.r);
    public static Hex operator -(Hex a, Hex b) => a + (-b);
    public static Hex operator *(Hex a, int d) => new Hex(a.q * d, a.r * d);
    public static Hex operator *(int d, Hex a) => a * d;
    public static bool operator ==(Hex a, Hex b) => a.Equals(b);
    public static bool operator !=(Hex a, Hex b) => !a.Equals(b);

    private static Hex Round(float q, float r) => new Hex(Mathf.RoundToInt(q), Mathf.RoundToInt(r));
}
