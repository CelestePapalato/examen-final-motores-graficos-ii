using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerInstancing : MonoBehaviour
{
    [SerializeField]
    Vector3 offset = new Vector3(0, 0.05f, 0);
    [SerializeField]
    CinemachineTargetGroup targetGroup;
    [SerializeField]
    List<Transform> startingPoints = new List<Transform>();

    List<Character> loadedCharaters = new List<Character>();

    void Start()
    {
        InstanceCharacters();
    }

    private void OnEnable()
    {
        LevelManager.Instance?.OnSceneLoad.AddListener(DestroyCharacters);
    }

    private void OnDisable()
    {
        LevelManager.Instance?.OnSceneLoad?.RemoveListener(DestroyCharacters);
    }

    void InstanceCharacters()
    {
        PlayerInput[] players = PlayerManager.CurrentPlayers;
        Dictionary<PlayerInput, Character> characters = PlayerManager.CurrentCharacters;
        if (startingPoints.Count < 1 ||
            players.Length < 1 ||
            characters.Count != players.Length) { return; }

        for (int i = 0; i < players.Length; i++)
        {
            Vector3 pos = startingPoints[i].position;
            Quaternion rotation = startingPoints[i].rotation;
            PlayerInput p = players[i];
            p.transform.position = pos;
            Character character = Instantiate(characters[p], p.transform);
            character.transform.rotation = rotation;
            character.transform.position += offset;
            targetGroup.AddMember(character.MovementComponent.transform, 1, 1);
            Player player = p.GetComponent<Player>();
            player.enabled = true;
            loadedCharaters.Add(character);
        }
    }

    void DestroyCharacters()
    {
        for(int i = 0; i < loadedCharaters.Count; i++)
        {
            Destroy(loadedCharaters[i].gameObject);
        }
        loadedCharaters = null;
    }

}
