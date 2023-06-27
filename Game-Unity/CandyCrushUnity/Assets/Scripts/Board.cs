using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public sealed class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    [SerializeField] private AudioClip collectSound;
    [SerializeField] private AudioClip errorSound;

    [SerializeField] private AudioSource _audioSource;

    public Row[] rows;
    public Tile[,] Tiles { get; private set; }
    public int Width => Tiles.GetLength(dimension: 0);
    public int Height => Tiles.GetLength(dimension: 1);

    private readonly List<Tile> _selection = new List<Tile>();

    private const float TweenDuration = 0.25f; 

    private void Awake() => Instance = this;

    // Start is called before the first frame update
    private void Start()
    {
        Console.WriteLine(rows.Length);
        Console.WriteLine(rows.Max(selector: row => row.tiles.Length));
        Tiles = new Tile[rows.Max(selector: row => row.tiles.Length), rows.Length];

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = rows[y].tiles[x];
                tile.Item = ItemDatabase.Items[UnityEngine.Random.Range(0, ItemDatabase.Items.Length)];
                tile.x = x;
                tile.y = y;

                Tiles[x, y] = tile;
            }
        }

        while (CanPop())
            PopAsync(IsHuman: false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.A)) return;

        foreach (var connectedTile in Tiles[0, 0].GetConnectedTiles())
            connectedTile.icon.transform.DOScale(endValue: 1.25f, TweenDuration).Play();
    }


    public async void Select(Tile tile)
    {
        if (!_selection.Contains(tile))
            _selection.Add(tile);

        if (_selection.Count < 2) return;
        if (!_selection[0].Neighbours.Contains(_selection[1]))
        {
            _selection.Clear();
            return;
        }


        Debug.Log(message: $"Tile selected at {_selection[0].x}, {_selection[0].y}, and {_selection[1].x}, {_selection[1].y}");

        await SwapAsync(_selection[0], _selection[1]);

        if (CanPop())
        {
            PopAsync();
        }
        else
        {
            _audioSource.PlayOneShot(errorSound);
            await SwapAsync(_selection[0], _selection[1]);
        }

        _selection.Clear();
    }

    public async Task SwapAsync(Tile tile1, Tile tile2)
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();

        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration)
            .SetEase(Ease.OutQuad)) // Set easing for swap animation
            .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration)
            .SetEase(Ease.OutQuad)); // Set easing for swap animation

        await sequence.Play()
                      .AsyncWaitForCompletion();

        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        (tile2.Item, tile1.Item) = (tile1.Item, tile2.Item);
    }

    private bool CanPop()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (Tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2)
                    return true;
            }
        }

        return false;
    }

    private async void PopAsync(bool IsHuman=true)
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];

                var connectedTiles = tile.GetConnectedTiles();
                if (connectedTiles.Skip(1).Count() < 2)
                    continue;

                if (IsHuman)
                {
                    var deflateSequance = DOTween.Sequence();

                    foreach (var connectedTile in connectedTiles)
                    {
                        deflateSequance.Join(connectedTile.icon.transform.DOScale(Vector3.zero, TweenDuration))
                                       .SetEase(Ease.InQuad);
                    }

                    _audioSource.PlayOneShot(collectSound);

                    await deflateSequance.Play()
                                         .AsyncWaitForCompletion();

                    ScoreCounter.Instance.Score += tile.Item.score * connectedTiles.Count();

                    
                }

                var inflateSequence = DOTween.Sequence();
                foreach (var connectedTile in connectedTiles)
                {
                    connectedTile.Item = ItemDatabase.Items[UnityEngine.Random.Range(0, ItemDatabase.Items.Length)];
                    if (IsHuman)
                    {
                        inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration))
                                       .SetEase(Ease.InQuad);
                    }
                }

                if (IsHuman)
                {
                    await inflateSequence.Play()
                                         .AsyncWaitForCompletion();
                }

                x = 0;
                y = 0;
            }
        }
    }
}