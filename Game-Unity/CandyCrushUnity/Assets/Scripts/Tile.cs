using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public sealed class Tile : MonoBehaviour
{
    public int x, y;

    private Item _item;
    public Image icon;
    public Button button;

    public Item Item
    {
        get => _item;
        set
        {
            if (_item == value) return;
            _item = value;

            icon.sprite = _item.sprite;
        }
    }

    public Tile Left => x > 0 ? Board.Instance.Tiles[x - 1, y] : null;
    public Tile Top => y > 0 ? Board.Instance.Tiles[x, y - 1] : null;
    public Tile Right => x < Board.Instance.Width - 1 ? Board.Instance.Tiles[x + 1, y] : null;
    public Tile Bottom => y < Board.Instance.Height - 1 ? Board.Instance.Tiles[x, y + 1] : null;

    public Tile[] Neighbours => new[]
    {
        Left,
        Top,
        Right,
        Bottom
    };

    private void Start()
    {
        button.onClick.AddListener(call: () => Board.Instance.Select(tile: this));
    }

    public List<Tile> GetConnectedTiles(List<Tile> exclude = null, int? row = null, int? col = null)
    {
        // DFS (death-first search) algorrithm

        var result = new List<Tile>() { this };

        exclude ??= new List<Tile>();
        row ??= this.x;
        col ??= this.y;

        exclude.Add(this);

        foreach (var neighbour in Neighbours)
        {
            if (neighbour == null || exclude.Contains(neighbour) || neighbour.Item != Item)
                continue;

            // Check if the neighbour is in the same row or column 
            if (row == neighbour.x || col == neighbour.y)              // Bug: result contatins L shaped tiles too
            {
                result.AddRange(neighbour.GetConnectedTiles(exclude, x, y));
            }
        }

        return result;
    }
}
