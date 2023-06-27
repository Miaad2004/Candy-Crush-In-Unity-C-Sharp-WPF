using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;

public class LoginSystemConnectionHandeler : MonoBehaviour
{
    public static LoginSystemConnectionHandeler Instance { get; private set; }

    private void Awake() => Instance = this;

    public string MatchResultOutputPath { get; private set; } = $"./matchResult.json";

    void Start()
    {
        float gameDurationInSeconds = 75f;
        string[] commandLineArgs = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < commandLineArgs.Length; i++)
        {
            if (commandLineArgs[i] == "-gameDuration" && i + 1 < commandLineArgs.Length)
            {
                gameDurationInSeconds = float.Parse(commandLineArgs[i + 1]);
            }

            else if (commandLineArgs[i] == "-matchResultOutputPath" && i + 1 < commandLineArgs.Length)
            {
                MatchResultOutputPath = commandLineArgs[i + 1];
            }
        }

        GameTimer.Instance.TotalTimeInSeconds = gameDurationInSeconds;
        Debug.Log($"Total Game Time: {GameTimer.Instance.TotalTimeInSeconds}");
    }

    public async Task SendScoreToWPF()
    {
        var finalScore = ScoreCounter.Instance.Score;
        var scoreData = new { Score = finalScore };

        var json = JsonConvert.SerializeObject(scoreData);
        await File.WriteAllTextAsync(MatchResultOutputPath, json);
    }
}
