using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

[JsonObject(IsReference = false)]
public struct IntVector2
{
    public int x;
    public int y;

    public static IntVector2 Zero  = new IntVector2( 0,  0);
    public static IntVector2 One   = new IntVector2( 1,  1);

    public static IntVector2 Left  = new IntVector2(-1,  0);
    public static IntVector2 Right = new IntVector2( 1,  0);
    public static IntVector2 Up    = new IntVector2( 0,  1);
    public static IntVector2 Down  = new IntVector2( 0, -1);

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public IntVector2(float x, float y)
        : this((int)x, (int)y)
    {
    }

    public IntVector2(Vector2 point)
        : this(point.x, point.y)
    {
    }

    public static implicit operator Vector2(IntVector2 point)
    {
        return new Vector2(point.x, point.y);
    }

    public static implicit operator Vector3(IntVector2 point)
    {
        return new Vector3(point.x, point.y, 0);
    }

    public static implicit operator IntVector2(Vector2 vector)
    {
        return new IntVector2(vector);
    }

    public static implicit operator IntVector2(Vector3 vector)
    {
        return new IntVector2(vector);
    }

    public override bool Equals(object obj)
    {
        if (obj is IntVector2)
        {
            return Equals((IntVector2)obj);
        }

        return false;
    }

    public bool Equals(IntVector2 other)
    {
        return other.x == x
            && other.y == y;
    }

    public override int GetHashCode()
    {
        return new { x, y }.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("IntVector2({0}, {1})", x, y);
    }

    public static bool operator ==(IntVector2 a, IntVector2 b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(IntVector2 a, IntVector2 b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
    {
        a.x += b.x;
        a.y += b.y;

        return a;
    }

    public static IntVector2 operator -(IntVector2 a, IntVector2 b)
    {
        a.x -= b.x;
        a.y -= b.y;

        return a;
    }

    public static IntVector2 operator *(IntVector2 a, int scale)
    {
        a.x *= scale;
        a.y *= scale;

        return a;
    }

    public static IntVector2 operator *(IntVector2 a, float scale)
    {
        a.x = Mathf.RoundToInt(a.x * scale);
        a.y = Mathf.RoundToInt(a.y * scale);

        return a;
    }
}
