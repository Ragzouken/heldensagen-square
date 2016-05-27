using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour 
{
    private struct Cell
    {
        public IntVector2 position;
        public Sprite sprite;
        public Color color;
        public int rotation;
    }

    public Color light, dark;
    public Sprite square, border;

    public Transform cellParent;
    public Image cellPrefab;

    private MonoBehaviourPooler<Cell, Image> test_cells;

    private void Awake()
    {
        test_cells = new MonoBehaviourPooler<Cell, Image>(cellPrefab,
                                                          cellParent,
                                                          InitTestCell);
    }

    private void InitTestCell(Cell cell, Image image)
    {
        image.transform.localPosition = cell.position * 16;
        image.transform.localRotation = Quaternion.Euler(0, 0, 90 * -cell.rotation);
        image.sprite = cell.sprite;
        image.color = cell.color;
    }

    private static IntVector2[] directions =
    {
        IntVector2.Right,
        IntVector2.Down,
        IntVector2.Left,
        IntVector2.Up,
    };

    private IEnumerator Start()
    {
        while (true)
        {
            Reset();

            yield return new WaitForSeconds(2);
        }
    }

    private IEnumerable<Cell> Edges(Dictionary<IntVector2, Shape.Cell> cells)
    {
        foreach (var cell in cells.Keys)
        {
            for (int d = 0; d < 4; ++d)
            {
                if (!cells.ContainsKey(cell + directions[d]))
                {
                    yield return new Cell
                    {
                        position = cell,
                        rotation = d,
                        color = light,
                        sprite = border,
                    };
                }
            }
        }
    }

    private void Update()
    {
        int r = Mathf.FloorToInt(Time.timeSinceLevelLoad * 4) % 4;
        var oriented = shape.GetOriented(IntVector2.Zero, r);

        var things = oriented.Keys.Select(p => new Cell
        {
            position = p,
            color = dark,
            sprite = square,
        });

        things = things.Concat(Edges(oriented));

        test_cells.SetActive(things, sort: true);
    }

    private Shape shape;

    private void Reset()
    {
        var shape = new Shape();
        var current = IntVector2.Zero;

        var min = IntVector2.Zero;
        var max = IntVector2.Zero;

        for (int i = 0; i < 7; ++i)
        {
            shape.cells[current] = Shape.Cell.Blank;

            min.x = Mathf.Min(min.x, current.x);
            min.y = Mathf.Min(min.y, current.y);
            max.x = Mathf.Max(max.x, current.x);
            max.y = Mathf.Max(max.y, current.y);

            var direction = IntVector2.Zero;

            while (shape.cells.ContainsKey(current + direction))
            {
                direction = directions[Random.Range(0, 4)];
            }

            current += direction;
        }

        var center = new IntVector2((min.x + max.x) / 2,
                                    (min.y + max.y) / 2);

        this.shape = new Shape();

        foreach (var pair in shape.cells)
        {
            this.shape.cells[pair.Key - center] = pair.Value;
        }
    }
}
