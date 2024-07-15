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
            healthComponent.onHealthUpdate -= UpdateSlider;
            healthComponent = value;
            if(healthComponent)
            {
                healthComponent.onHealthUpdate += UpdateSlider;
            }
            else
            {
                healthComponent.onHealthUpdate -= UpdateSlider;
            }
            healthbarSlider?.gameObject.SetActive(healthComponent);
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
            healthComponent.onHealthUpdate += UpdateSlider;
        }
    }

    private void OnDisable()
    {
        if (healthComponent)
        {
            healthComponent.onHealthUpdate -= UpdateSlider;
        }
    }

    public void UpdateSlider(int value, int maxValue)
    {
        float slider_value = (float) value / maxValue;
        healthbarSlider.value = slider_value;
    }
}
