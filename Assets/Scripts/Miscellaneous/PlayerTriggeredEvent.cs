using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerTriggeredEvent : MonoBehaviour
{
    Dictionary<PlayerInput, Character> playersWhoHaveCrossed = new Dictionary<PlayerInput, Character>();
    PlayerInput[] playersInGame;

    public UnityEvent OnPlayerEnter;
    public UnityEvent OnPlayerExit;
    public UnityEvent OnAllPlayersCrossed;

    private void Start()
    {
        playersInGame = PlayerManager.CurrentPlayers;
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();
        PlayerInput[] crossed = playersWhoHaveCrossed.Keys.ToArray();
        PlayerInput[] remain = playersInGame.Where(x => !crossed.Contains(x)).ToArray();

        var obj = remain.FirstOrDefault(x => x.GetComponentInChildren<Character>() == character);

        if (obj != null)
        {
            playersWhoHaveCrossed.Add(obj, character);
        }

        if(playersWhoHaveCrossed.Count == playersInGame.Length)
        {
            OnAllPlayersCrossed?.Invoke();
        }
        else
        {
            OnPlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();
        PlayerInput[] crossed = playersWhoHaveCrossed.Keys.ToArray();

        PlayerInput playerLeft = crossed.FirstOrDefault(x =>x.GetComponentInChildren<Character>() == character);

        if (playerLeft != null)
        {
            playersWhoHaveCrossed.Remove(playerLeft);
            OnPlayerExit?.Invoke();
        }
    }
}
