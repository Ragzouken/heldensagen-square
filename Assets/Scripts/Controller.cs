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
        public int depth;
    }

    public Fleet[] fleets;

    public new CameraController camera;
    public Color back;
    public Sprite square, border, grid;

    public Transform cellParent;
    public SpriteRenderer cellPrefab;

    private MonoBehaviourPooler<Cell, SpriteRenderer> test_cells;

    public FleetCountPanel fleetCountPrefab;
    public Transform fleetCountParent;

    private MonoBehaviourPooler<Fleet, FleetCountPanel> fleetCounts;    

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
        test_cells = new MonoBehaviourPooler<Cell, SpriteRenderer>(cellPrefab,
                                                                   cellParent,
                                                                   InitTestCell);

        fleetCounts = new MonoBehaviourPooler<Fleet, FleetCountPanel>(fleetCountPrefab,
                                                                      fleetCountParent,
                                                                      (f, p) => p.SetFleet(f));
    }

    private void InitTestCell(Cell cell, SpriteRenderer image)
    {
        image.transform.localPosition = ((Vector3) (cell.position * 16 + IntVector2.One * 8)) + Vector3.back * cell.depth;
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

    private IEnumerable<Cell> Edges(Play play, int depth)
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
                        color = play.fleet.light,
                        sprite = border,
                        depth = depth,
                    };
                }
            }
        }
    }

    private HashSet<IntVector2> covered = new HashSet<IntVector2>();
    private List<Cell> cells = new List<Cell>();

    private int turn = 0;

    private void Refresh()
    {
        covered.Clear();
        cells.Clear();

        var fleet = fleets[turn];

        int i = plays.Count * 2;

        var position = camera.ScreenToWorld(Input.mousePosition) / 16;

        position.x = Mathf.Floor(position.x);
        position.y = Mathf.Floor(position.y);

        var test = plays.Concat(new[] { new Play(fleet, shape, position, rotation) });

        foreach (Play play in test.Reverse<Play>())
        {
            i -= 1;

            var uncovered = play.cells.Keys.Where(p => !covered.Contains(p));

            cells.AddRange(Edges(play, i * 2 + 1).Where(c => !covered.Contains(c.position)));
            cells.AddRange(uncovered.Select(p => new Cell
            {
                position = p,
                color = play.fleet.dark,
                sprite = square,
                depth = i * 2,
            }));

            covered.UnionWith(uncovered);
        }

        test_cells.SetActive(cells.Reverse<Cell>(), sort: false);

        fleetCounts.SetActive(fleets);
        fleetCounts.MapActive((f, p) => p.Refresh());
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
        var fleet = fleets[turn];

        var position = camera.ScreenToWorld(Input.mousePosition) / 16;

        position.x = Mathf.Floor(position.x);
        position.y = Mathf.Floor(position.y);

        plays.Add(new Play(fleet, shape, position, rotation));

        turn = (turn + 1) % fleets.Length;

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
