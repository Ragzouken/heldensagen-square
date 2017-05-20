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
    public Sprite square, border, grid, connect;

    public List<Sprite> healthSprites;

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

        foreach (Fleet fleet in fleets)
        {
            var square = new Shape();
            square.cells.Add(IntVector2.Zero, Shape.Cell.Connect);

            var position = new IntVector2(Random.Range(-4, 5), Random.Range(-4, 5));

            plays.Add(new Play(fleet, square, position, 0));
        }
    }

    private void Update()
    {
        if (!overUI)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Play();
                Randomise();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                RotateRight();
            }
        }

        

        Refresh();
    }

    private IEnumerable<Cell> Edges(Play play, int depth, Dictionary<IntVector2, Fleet> covered)
    {
        foreach (var cell in play.cells.Keys.Where(c => !covered.ContainsKey(c)))
        {
            //if (play.cells[cell] == Shape.Cell.Connect)
            {
                int test = Mathf.Abs((cell.x + 23 * cell.y * 17) % 4);

                yield return new Cell
                {
                    position = cell,
                    color = play.fleet.light,
                    sprite = healthSprites[test],
                    depth = depth,
                };
            }

            ///*
            for (int d = 0; d < 4; ++d)
            {
                if (!play.cells.ContainsKey(cell + directions[d])
                 && !covered.ContainsKey(cell + directions[d]))
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
            //*/
        }
    }

    private Dictionary<IntVector2, Fleet> covered = new Dictionary<IntVector2, Fleet>();
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

        bool first = true;

        foreach (Play play in test.Reverse<Play>())
        {
            bool invalid = first && !CanPlay(play);

            i -= 1;

            var uncovered = play.cells.Keys.Where(p => !covered.ContainsKey(p));

            cells.AddRange(Edges(play, i * 2 + 1, covered).Where(c => !covered.ContainsKey(c.position)));
            cells.AddRange(uncovered.Select(p => new Cell
            {
                position = p,
                color = invalid ? Color.gray : play.fleet.dark,
                sprite = square,
                depth = i * 2,
            }));

            foreach (var cell in uncovered) covered.Add(cell, play.fleet);

            first = false;
        }

        test_cells.SetActive(cells.Reverse<Cell>(), sort: false);

        foreach (Fleet fleet_ in fleets)
        {
            fleet_.ships = covered.Count(pair => pair.Value == fleet_);
        }

        fleetCounts.SetActive(fleets);
        fleetCounts.MapActive((f, p) => p.Refresh());
    }

    public bool CanPlay(Play play)
    {
        var covered = new Dictionary<IntVector2, Fleet>();
        var symbol = new Dictionary<IntVector2, Shape.Cell>();

        foreach (Play play_ in plays.Reverse<Play>())
        {
            foreach (IntVector2 cell in play_.cells.Keys)
            {
                if (!covered.ContainsKey(cell))
                {
                    covered[cell] = play_.fleet;
                    symbol[cell] = play_.cells[cell];
                }
            }
        }

        return play.cells.Keys.Any(c => covered.ContainsKey(c) && covered[c] == play.fleet && symbol[c] == Shape.Cell.Connect);
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

        for (int i = 0; i < 2; ++i)
        {
            var key = shape.cells.Keys.RandomElement();
            shape.cells[key] = Shape.Cell.Connect;
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

        var play = new Play(fleet, shape, position, rotation);

        if (!CanPlay(play))
            return;

        plays.Add(play);

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
