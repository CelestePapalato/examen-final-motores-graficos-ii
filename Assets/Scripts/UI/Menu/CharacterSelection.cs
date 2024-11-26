using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CharacterSelection : MonoBehaviour
{
    [Serializable]
    public class CharacterData
    {
        public string name_tag;
        public Character characterPrefab;
    }

    [SerializeField]
    TMPro.TMP_Text playerText;
    [SerializeField]
    CharacterData[] characterData;

    public UnityEvent<Character> OnCharacterSelected;
    public UnityEvent OnSelectionFinished;

    public void AddCharacterToPlayer(string tag)
    {
        CharacterData data = characterData.First(characterData => characterData.name_tag == tag);
        if (data == null) { return; }
        OnCharacterSelected?.Invoke(data.characterPrefab);
        UpdatePlayerText();
    }

    private void UpdatePlayerText()
    {
        if(PlayerManager.PlayerIDsWithoutCharacters.Length == 0)
        {
            OnSelectionFinished?.Invoke();
            return;
        }

        if (playerText)
        {
            int currentPlayer = PlayerManager.PlayerIDsWithoutCharacters[0] + 1;
            playerText.text = "Player " + currentPlayer;
        }
    }

    private void OnEnable()
    {
        UpdatePlayerText();
    }
}
