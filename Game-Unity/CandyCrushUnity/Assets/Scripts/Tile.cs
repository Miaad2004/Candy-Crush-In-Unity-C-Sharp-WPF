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
        StartFloatingAnimation();
    }


    public List<Tile> GetConnectedTiles(List<Tile> exclude = null, int? row = null, int? col = null)
    {
        var result = new List<Tile>() { this };

        exclude ??= new List<Tile>();
        row ??= this.x;
        col ??= this.y;

        exclude.Add(this);

        foreach (var neighbour in Neighbours)
        {
            if (neighbour == null || exclude.Contains(neighbour) || neighbour.Item != Item)
                continue;

            // Check if the neighbour is in the same row or column and not diagonal
            if ((row == neighbour.x || col == neighbour.y) && (row != neighbour.x || col != neighbour.y))
            {
                result.AddRange(neighbour.GetConnectedTiles(exclude, neighbour.x, neighbour.y));
            }
        }

        return result;
    }



    public void StartFloatingAnimation()
    {
        Vector3 initialPosition = icon.transform.localPosition;
        Vector3 targetPosition = initialPosition + new Vector3(0f, 0.2f, 0f); // Offset the Y position for the floating effect

        icon.transform.DOKill(); // Cancel any ongoing animations on the icon

        // Start the floating animation
        icon.transform.localPosition = initialPosition; // Reset the position to the initial position
        icon.transform.DOLocalMove(targetPosition, 1f)
            .SetLoops(-1, LoopType.Yoyo) // Makes the animation loop back and forth
            .SetEase(Ease.InOutQuad); // Sets the easing function for smooth movement
    }
}
