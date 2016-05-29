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

    [System.Serializable]
    public class Scheme
    {
        public Color light, dark;
    }

    public Scheme[] scemes;

    public new CameraController camera;
    public Color back;
    public Sprite square, border, grid;

    public Transform cellParent;
    public Image cellPrefab;

    private MonoBehaviourPooler<Cell, Image> test_cells;

    private List<Play> plays = new List<Play>();

    private int rotation;
    private Shape shape;

    public static bool overUI
    {
        get
        {
            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }
    }

    private void Awake()
    {
        test_cells = new MonoBehaviourPooler<Cell, Image>(cellPrefab,
                                                          cellParent,
                                                          InitTestCell);
    }

    private void InitTestCell(Cell cell, Image image)
    {
        image.transform.localPosition = cell.position * 16 + IntVector2.One * 8;
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

    private void Start()
    {
        Randomise();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !overUI)
        {
            Play();
            Randomise();
        }

        Refresh();
    }

    private IEnumerable<Cell> Edges(Play play, Scheme scheme)
    {
        foreach (var cell in play.cells.Keys)
        {
            for (int d = 0; d < 4; ++d)
            {
                if (!play.cells.ContainsKey(cell + directions[d]))
                {
                    yield return new Cell
                    {
                        position = cell,
                        rotation = d,
                        color = scheme.light,
                        sprite = border,
                    };
                }
            }
        }
    }

    private void Refresh()
    {
        /*
        var things = Enumerable.Range(-4, 9).SelectMany(x => Enumerable.Range(-4, 9), (x, y) => new Cell
        {
            position = new IntVector2(x, y),
            color = back,
            sprite = grid,
        });
        */

        var covered = new HashSet<IntVector2>();
        var cells = new List<Cell>();

        int i = plays.Count;

        var position = camera.ScreenToWorld(Input.mousePosition) / 16;

        position.x = Mathf.Floor(position.x);
        position.y = Mathf.Floor(position.y);

        var test = plays.Concat(new[] { new Play(shape, position, rotation) });

        foreach (Play play in test.Reverse<Play>())
        {
            Scheme scheme = scemes[i-- % scemes.Length];

            var uncovered = play.cells.Keys.Where(p => !covered.Contains(p));

            cells.AddRange(Edges(play, scheme).Where(c => !covered.Contains(c.position)));
            cells.AddRange(uncovered.Select(p => new Cell
            {
                position = p,
                color = scheme.dark,
                sprite = square,
            }));

            covered.UnionWith(uncovered);
        }

        test_cells.SetActive(cells.Reverse<Cell>(), sort: true);
    }

    private void Randomise()
    {
        shape = new Shape();
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

        var center = new IntVector2(Mathf.RoundToInt((min.x + max.x) / 2f),
                                    Mathf.RoundToInt((min.y + max.y) / 2f));

        var copy = new Dictionary<IntVector2, Shape.Cell>(shape.cells);

        shape.cells.Clear();

        foreach (var pair in copy)
        {
            shape.cells[pair.Key - center] = pair.Value;
        }
    }

    private void Play()
    {
        var position = camera.ScreenToWorld(Input.mousePosition) / 16;

        position.x = Mathf.Floor(position.x);
        position.y = Mathf.Floor(position.y);

        plays.Add(new Play(shape, position, rotation));

        Refresh();
    }

    public void RotateRight()
    {
        rotation = (rotation + 1) % 4;
    }

    public void RotateLeft()
    {
        rotation = (rotation + 4 - 1) % 4;
    }
}
