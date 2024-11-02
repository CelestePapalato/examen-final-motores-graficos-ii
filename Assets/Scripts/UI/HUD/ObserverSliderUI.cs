using UnityEngine;
using UnityEngine.UI;

public class ObserverSliderUI : MonoBehaviour
{
    private IObservableVariable variable;
    public IObservableVariable ObservableVariable
    {
        get => variable;
        set
        {
            if (variable != null)
            {
                variable.OnUpdate -= UpdateSlider;
            }
            variable = value;
            if(variable != null)
            {
                variable.OnUpdate += UpdateSlider;
            }
            else
            {
                variable.OnUpdate -= UpdateSlider;
            }
            slider?.gameObject.SetActive(variable != null);
            UpdateSlider(variable.Current, variable.Max);
        }
    }

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.gameObject.SetActive(!slider || variable == null);
    }

    private void OnEnable()
    {
        if (variable != null)
        {
            variable.OnUpdate += UpdateSlider;
        }
    }

    private void OnDisable()
    {
        if (variable != null)
        {
            variable.OnUpdate -= UpdateSlider;
        }
    }

    public void UpdateSlider(int value, int maxValue)
    {
        float slider_value = (float) value / maxValue;
        slider.value = slider_value;
    }
}
