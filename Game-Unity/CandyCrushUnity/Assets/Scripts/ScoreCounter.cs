using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;

    private int _score;

    public int Score
    {
        get => _score;

        set
        {
            if (_score == value) return;

            _score = value;

            scoreText.SetText($"Score: {_score}");
            Debug.Log(value);
            Debug.Log(scoreText);
        }
    }

    

    private void Awake() => Instance = this;

}
