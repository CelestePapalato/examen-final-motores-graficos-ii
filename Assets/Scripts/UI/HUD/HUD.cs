using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField]
    ObserverSliderUI healthbar;
    [SerializeField]
    ObserverSliderUI manabar;

    private Character m_Character = null;
    public Character Character { get => m_Character; set => InitializeSliders(value); }

    private void InitializeSliders(Character character)
    {
        if(character == null)
        {
            healthbar.ObservableVariable = null;
            manabar.ObservableVariable = null;
            m_Character = null;
            return;
        }
        if (m_Character != character)
        {
            healthbar.ObservableVariable = character.Health.GetComponent<IObservableVariable>();
            manabar.ObservableVariable = character.Mana.GetComponent<IObservableVariable>();
            m_Character = character;
        }
    }
}
