using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Match-3/Item")]
public sealed class Item : ScriptableObject
{
    public int score; 
    public Sprite sprite;      // tile's image
}
