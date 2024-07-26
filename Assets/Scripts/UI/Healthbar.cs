using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Health healthComponent;
    public Health HealthComponent
    {
        get => healthComponent;
        set
        {
            if (healthComponent)
            {
                healthComponent.OnHealthUpdate -= UpdateSlider;
            }
            healthComponent = value;
            if(healthComponent)
            {
                healthComponent.OnHealthUpdate += UpdateSlider;
            }
            else
            {
                healthComponent.OnHealthUpdate -= UpdateSlider;
            }
            healthbarSlider?.gameObject.SetActive(healthComponent);
            UpdateSlider(healthComponent.CurrentHealth, healthComponent.MaxHealth);
        }
    }

    private Slider healthbarSlider;

    private void Awake()
    {
        healthbarSlider = GetComponent<Slider>();
        healthbarSlider.gameObject.SetActive(!healthbarSlider || !healthComponent);
    }

    private void OnEnable()
    {
        if (healthComponent)
        {
            healthComponent.OnHealthUpdate += UpdateSlider;
        }
    }

    private void OnDisable()
    {
        if (healthComponent)
        {
            healthComponent.OnHealthUpdate -= UpdateSlider;
        }
    }

    public void UpdateSlider(int value, int maxValue)
    {
        float slider_value = (float) value / maxValue;
        healthbarSlider.value = slider_value;
    }
}
