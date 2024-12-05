using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Serializable]
    private class Screen
    {
        public string name;
        public bool activeOnStart = false;
        public Menu menu;
    }

    [SerializeField]
    Screen[] screens;

    private void Awake()
    {
        if(Instance != this && Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        foreach (Screen screen in screens)
        {
            if(screen.activeOnStart)
            {
                screen.menu.Open();
            }
            else
            {
                screen.menu.Close();
            }
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += ShowResults;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ShowResults;
    }

    private void ShowResults(bool nivelCompletado)
    {
        Screen resultsScreen = screens.First(x => x.name == "results");
        if (resultsScreen != null)
        {
            Menu menu = resultsScreen.menu;
            menu.Open();
            Debug.Log("Menu Manager: " + resultsScreen.name + " menu opened");
            menu.Initialize(nivelCompletado);
        }
    }

    public void OpenMenu(string name)
    {
        Screen screen = screens.First(x => x.name == name);
        if(screen != null)
        {
            screen.menu?.Open();
            Debug.Log("Menu Manager: " + name + " menu opened");
        }
    }

    public void CloseMenu(string name)
    {
        Screen screen = screens.First(x => x.name == name);
        if (screen != null)
        {
            screen.menu?.Close();
            Debug.Log("Menu Manager: " + name + " menu closed");
        }
    }

    public void CloseAllScreens()
    {
        foreach(var screen in screens)
        {
            screen.menu?.Close();
            Debug.Log("Menu Manager: " + name + " menu closed");
        }
    }

    public void AlternateState(string name)
    {
        Screen screen = screens.First(x => x.name == name);
        if (screen != null)
        {
            if (screen.menu.Closed)
            {
                screen.menu?.Open();
                Debug.Log("Menu Manager: " + name + " menu opened");
            }
            else
            {
                screen.menu?.Close();
                Debug.Log("Menu Manager: " + name + " menu closed");
            }
        }
    }
}
