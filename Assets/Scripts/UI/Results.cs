using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Results : MonoBehaviour
{
    [Header("Ganar")]
    [SerializeField] Canvas winCanvas;
    [SerializeField] TMP_Text scoreText;
    [Header("Perder")]
    [SerializeField] Canvas lostCanvas;

    private void Awake()
    {
        winCanvas?.gameObject.SetActive(false);
        lostCanvas?.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += ShowResults;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ShowResults;
    }

    public void ShowResults(bool nivelCompletado)
    {
        if(nivelCompletado)
        {
            winCanvas?.gameObject.SetActive(true);
            if (scoreText)
            {
                scoreText.text = GameManager.Instance.Score + "";
            }
        }
        else
        {
            lostCanvas?.gameObject.SetActive(true);
        }
    }
}
