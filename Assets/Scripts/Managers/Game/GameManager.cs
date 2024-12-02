using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static UnityAction<int> OnScoreUpdate;

    public static UnityAction<bool> OnGameOver;

    private int score = 0;
    public int Score { get => score; }

    public static float GameTime { get => Time.timeSinceLevelLoad; }

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
    }

    private void OnEnable()
    {
        EnemyAI.OnEnemyDead += ScoreUp;
        Points.OnCollected += ScoreUp;
        Player.OnPlayerDead += GameOver;
    }

    private void OnDisable()
    {
        EnemyAI.OnEnemyDead -= ScoreUp;
        Points.OnCollected -= ScoreUp;
        Player.OnPlayerDead -= GameOver;
    }

    private void Start()
    {
        score = 0;
        OnScoreUpdate?.Invoke(score);
    }

    private void ScoreUp(int puntos)
    {
        puntos = Mathf.Max(puntos, 0);
        score += puntos;
        OnScoreUpdate?.Invoke(score);
    }

    private void GameOver(Player player)
    {
        Player[] alive = Player.CurrentPlayers.Where(x => !x.IsDead).ToArray();
        if (alive.Length > 0)
        {
            return;
        }
        if (Checkpoint.TryRespawning())
        {
            return;
        }
        Time.timeScale = 0;
        OnGameOver?.Invoke(false);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Cambiar lógica para cuando termine la pantalla de resultados
    }

    public void LevelCompleted()
    {
        Time.timeScale = 0;
        OnGameOver?.Invoke(true);
    }
}
