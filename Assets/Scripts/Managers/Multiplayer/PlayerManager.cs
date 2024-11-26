using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public static event Action<PlayerInput> onPlayerAdded;
    public UnityEvent onFirstPlayerJoined;
    public UnityEvent onMultiplayer;
    public UnityEvent onMultiplayerDisabled;
    public UnityEvent onNoPlayersLeft;

    public static PlayerInput[] CurrentPlayers { get => players.ToArray(); }
    public static int[] PlayerIDsWithoutCharacters { get => playerIDsWithNoCharacters.ToArray(); }

    private static List<PlayerInput> players = new List<PlayerInput>();
    private static Dictionary<PlayerInput, Character> playerCharacter = new Dictionary<PlayerInput, Character>();
    private static List<int> playerIDsWithNoCharacters = new List<int>();

    [SerializeField]
    private List<Transform> startingPoints;

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        if(Instance != this && Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
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

    private void OnDestroy()
    {
        Instance = null;
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

        playerIDsWithNoCharacters.Add(players.Count - 1);
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

    public void AddCharacterToPlayer(Character character)
    {
        if(playerIDsWithNoCharacters.Count > 0)
        {
            PlayerInput player = players[playerIDsWithNoCharacters[0]];
            playerCharacter.Add(player, character);
            playerIDsWithNoCharacters.RemoveAt(0);
        }
    }

    public void EraseCharacters()
    {
        playerCharacter.Clear();
        playerIDsWithNoCharacters.Clear();
        for (int i = 0; i < players.Count; i++)
        {
            playerIDsWithNoCharacters.Add(i);
        }
    }

    public void ExpulsePlayers()
    {
        if(players.Count <= 1) { return; }
        PlayerInput[] playersJoined = CurrentPlayers;
        for(int i = 1; i < playersJoined.Length; i++) 
        {
            Destroy(playersJoined[i].gameObject);
        }
        playerIDsWithNoCharacters.Clear();
        playerIDsWithNoCharacters.Add(0);
    }
}