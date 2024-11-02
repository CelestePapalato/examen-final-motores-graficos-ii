using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static event Action<PlayerInput> onPlayerAdded;
    public UnityEvent onFirstPlayerJoined;
    public UnityEvent onMultiplayer;
    public UnityEvent onMultiplayerDisabled;
    public UnityEvent onNoPlayersLeft;

    public static PlayerInput[] CurrentPlayers { get => players.ToArray(); }
    private static List<PlayerInput> players = new List<PlayerInput>();

    [SerializeField]
    private List<Transform> startingPoints;

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        players.Clear();
    }

    private void OnEnable()
    {
        playerInputManager.playerJoinedEvent.AddListener(AddPlayer);
        playerInputManager.playerLeftEvent.AddListener(PlayerLeft);
    }

    private void OnDisable()
    {
        playerInputManager.playerJoinedEvent?.RemoveListener(AddPlayer);
        playerInputManager.playerLeftEvent?.RemoveListener(PlayerLeft);
    }

    private void AddPlayer(PlayerInput player)
    {
        players.Add(player);

        Debug.Log("Jugador " + players.Count + " ha entrado a la partida");

        onPlayerAdded?.Invoke(player);
        if (players.Count == 1)
        {
            onFirstPlayerJoined?.Invoke();
        }
        else
        {
            onMultiplayer?.Invoke();
        }
    }

    private void PlayerLeft(PlayerInput player)
    {
        players.Remove(player);
        if (players.Count == 1)
        {
            onMultiplayerDisabled?.Invoke();
        }

        if (players.Count == 0)
        {
            onNoPlayersLeft?.Invoke();
        }
    }
}