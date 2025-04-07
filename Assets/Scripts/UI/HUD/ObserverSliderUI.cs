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
                variable.OnDestroyEvent -= OnVariableDestroy;
            }
            if (onDestroy)
            {
                variable = null;
                return;
            }
            variable = value;
            if(variable != null)
            {
                variable.OnUpdate += UpdateSlider;
                variable.OnDestroyEvent += OnVariableDestroy;
                if (slider)
                {
                    slider.gameObject.SetActive(variable != null);
                }
                UpdateSlider(variable.Current, variable.Max);
            }
        }
    }

    private Slider slider;

    bool onDestroy = false;

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
            variable.OnDestroyEvent += OnVariableDestroy;
        }
    }

    private void OnDisable()
    {
        if (variable != null)
        {
            variable.OnUpdate -= UpdateSlider;
            variable.OnDestroyEvent -= OnVariableDestroy;
        }
    }

    private void OnDestroy()
    {
        onDestroy = true;
    }

    public void UpdateSlider(int value, int maxValue)
    {
        if (slider)
        {
            float slider_value = (float)value / maxValue;
            slider.value = slider_value;
        }
    }

    private void OnVariableDestroy()
    {
        variable = null;
    }
}
