using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Results : Menu
{
    [Header("Ganar")]
    [SerializeField] Canvas winCanvas;
    [SerializeField] TMP_Text scoreText;
    [Header("Perder")]
    [SerializeField] Canvas lostCanvas;

    public override void Initialize(bool nivelCompletado)
    {
        if(nivelCompletado)
        {
            winCanvas?.gameObject.SetActive(true);
            lostCanvas?.gameObject.SetActive(false);
            if (scoreText)
            {
                scoreText.text = GameManager.Instance.Score + "";
            }
        }
        else
        {
            lostCanvas?.gameObject.SetActive(true);
            winCanvas?.gameObject.SetActive(false);
        }
    }
}
