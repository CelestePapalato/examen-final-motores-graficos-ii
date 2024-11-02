using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField]
    ObserverSliderUI healthbar;
    [SerializeField]
    ObserverSliderUI manabar;

    private Character m_Character;
    public Character Character { get => m_Character; set => InitializeSliders(value); }

    private void InitializeSliders(Character character)
    {
        if (m_Character != character)
        {
            healthbar.ObservableVariable = character.Health.GetComponent<IObservableVariable>();
            manabar.ObservableVariable = character.Mana.GetComponent<IObservableVariable>();
            Debug.Log(character.Mana.gameObject.name);
        }
    }
}
