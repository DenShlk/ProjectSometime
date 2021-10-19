using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{
    public static int TopScore { get; private set; }

    public static void AddScore(int score)
    {
        TopScore = Mathf.Max(score, TopScore);
    }
}
