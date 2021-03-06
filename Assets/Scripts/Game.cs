﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public interface ICopyable<T>
{
    void Copy(Copier copier, T copy);
}

public class Copier : Dictionary<object, object>
{
    public T Copy<T>(T original) where T : ICopyable<T>, new()
    {
        object copy;

        if (!TryGetValue(original, out copy))
        {
            copy = new T();

            this[original] = copy;

            original.Copy(this, (T)copy);
        }

        return (T)copy;
    }
}

public class Game
{
}

public class Battle
{
    public Fleet[] fleets;

} 

[System.Serializable]
public class Fleet
{
    public string name;
    public int ships;
    public Color dark, light;
}

public class Play
{
    public Fleet fleet;
    public Shape shape;
    public IntVector2 position;
    public int rotation;

    public Dictionary<IntVector2, Shape.Cell> cells
        = new Dictionary<IntVector2, Shape.Cell>();

    public Play(Fleet fleet, Shape shape, IntVector2 position, int rotation)
    {
        this.fleet = fleet;

        foreach (var pair in shape.cells)
        {
            var pos = pair.Key;

            for (int r = rotation; r > 0; --r)
            {
                int x = pos.x;
                int y = pos.y;

                pos.x = y;
                pos.y = -x;
            }

            pos += position;

            cells[pos] = pair.Value;
        }
    }
}

public class Shape
{
    public enum Cell
    {
        None,
        Blank,
        Connect,
        Attack,
        Block,
    }

    public Dictionary<IntVector2, Cell> cells
        = new Dictionary<IntVector2, Cell>();
}
