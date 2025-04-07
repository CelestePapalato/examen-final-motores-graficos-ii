using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInstancing : MonoBehaviour
{
    [SerializeField]
    TransformFollower audioListenerPrefab;
    [SerializeField]
    Vector3 offset = new Vector3(0, 0.05f, 0);
    [SerializeField]
    CinemachineTargetGroup targetGroup;
    [SerializeField]
    List<Transform> startingPoints = new List<Transform>();
    List<Character> loadedCharaters = new List<Character>();
    List<TransformFollower> loadedAudioListeners = new List<TransformFollower>();

    void Start()
    {
        Player[] players = Player.CurrentPlayers;
        foreach (Player p in players)
        {
            p.enabled = false;
        }
        InstanceCharacters(startingPoints.ToArray());
    }

    private void OnEnable()
    {
        LevelManager.Instance?.OnSceneLoad.AddListener(DestroyCharacters);
        Checkpoint.OnRespawnSucceeded += RespawnCharacters;
    }

    private void OnDisable()
    {
        LevelManager.Instance?.OnSceneLoad?.RemoveListener(DestroyCharacters);
        Checkpoint.OnRespawnSucceeded -= RespawnCharacters;
    }

    void RespawnCharacters(Transform[] Points)
    {
        DestroyCharacters();
        InstanceCharacters(Points);
    }

    void InstanceCharacters(Transform[] Points)
    {
        PlayerInput[] players = PlayerManager.CurrentPlayers;
        Dictionary<PlayerInput, Character> characters = PlayerManager.CurrentCharacters;
        if (Points.Length < 1 ||
            players.Length < 1 ||
            characters.Count < players.Length) { return; }

        for (int i = 0; i < players.Length; i++)
        {
            Vector3 pos = Points[i].position;
            Quaternion rotation = Points[i].rotation;
            PlayerInput p = players[i];
            p.transform.position = pos;
            Character character = Instantiate(characters[p], p.transform);
            character.transform.rotation = rotation;
            character.transform.position += offset;
            targetGroup.AddMember(character.MovementComponent.transform, 1, 1);
            Player player = p.GetComponent<Player>();
            player.ChangeCharacter(character);
            player.enabled = true;
            loadedCharaters.Add(character);
            TransformFollower audioListener = Instantiate(audioListenerPrefab, p.transform);
            audioListener.Target = character.MovementComponent.transform;
            loadedAudioListeners.Add(audioListener);
        }
    }

    void DestroyCharacters()
    {
        for(int i = 0; i < loadedCharaters.Count; i++)
        {
            Character chara = loadedCharaters[i];
            Player player = chara.transform.parent.GetComponent<Player>();
            targetGroup.RemoveMember(chara.MovementComponent.transform);
            player.ChangeCharacter(null);
            player.enabled = false;
            TransformFollower audioListener = loadedAudioListeners[i];
            Destroy(audioListener.gameObject);
            Destroy(chara.gameObject);
        }
        loadedCharaters = new List<Character>();
        loadedAudioListeners = new List<TransformFollower>();
    }

}
