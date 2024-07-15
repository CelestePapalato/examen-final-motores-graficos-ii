using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    [SerializeField]
    TMP_Text scoreText;

    private void UpdateScore(int score)
    {
        if (!scoreText)
        {
            Debug.LogWarning("No se ha referenciado un componente Text para el puntaje.");
            return;
        }
        scoreText.text = score.ToString();
    }

    private void OnEnable()
    {
        GameManager.OnScoreUpdate += UpdateScore;
    }

    private void OnDisable()
    {
        GameManager.OnScoreUpdate -= UpdateScore;
    }
}
