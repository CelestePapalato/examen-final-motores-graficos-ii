using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static UnityAction<int> OnNewScore;
    public static UnityAction<int> OnNewMaxScore;

    public static UnityAction<bool> OnGameOver;

    private int score = 0;
    public int Score { get => score; }
    private static int maxScore = 0;
    public int MaxScore { get => maxScore; }

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
    }

    private void OnEnable()
    {
        //Enemigo.OnDead += SubirPuntuacion;
        Points.OnCollected += ScoreUp;
        Player.OnPlayerDead += GameOver;
    }

    private void OnDisable()
    {
        //Enemigo.OnDead -= ScoreUp;
        Points.OnCollected -= ScoreUp;
        Player.OnPlayerDead -= GameOver;
    }

    private void Start()
    {
        score = 0;
        OnNewScore?.Invoke(score);
        OnNewMaxScore?.Invoke(maxScore);
    }

    private void ScoreUp(int puntos)
    {
        puntos = Mathf.Max(puntos, 0);
        score += puntos;
        OnNewScore?.Invoke(score);
        if (score > maxScore)
        {
            maxScore = score;
            OnNewMaxScore?.Invoke(maxScore);
        }
    }

    private void GameOver(Player player)
    {
        Time.timeScale = 0;
        OnGameOver?.Invoke(false);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Cambiar l�gica para cuando termine la pantalla de resultados
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LevelCompleted()
    {
        Time.timeScale = 0;
        OnGameOver?.Invoke(true);
    }
}