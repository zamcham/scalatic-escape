using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BarUI : MonoBehaviour
{
    Slider slider;

    float currentValue;

    [SerializeField] float maxValue;
    [SerializeField] float valueLerpDuration = 0.25f;

    Coroutine lerpCoroutine;
    
    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = maxValue;
    }

    public void SetValue(float value)
    {
        if (currentValue == value) { return; }

        currentValue = value;
        currentValue = Mathf.Clamp(currentValue, 0f, maxValue);

        if (lerpCoroutine != null) { StopCoroutine(lerpCoroutine); }
        lerpCoroutine = StartCoroutine(ValueLerp(value, valueLerpDuration));
    }

    IEnumerator ValueLerp(float targetValue, float duration)
    {
        float lerp = 0f;
        float startingValue = slider.value;

        while (lerp < 1f)
        {
            lerp += Time.deltaTime / duration;

            slider.value = Mathf.Lerp(startingValue, targetValue, lerp);

            yield return null;
        }
    }
}