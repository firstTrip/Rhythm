using UnityEngine;
using UnityEngine.UI;

public class UINoteCircle : MonoBehaviour
{
    private RectTransform _rect;
    private float _targetTime;
    private Image _image;

    private float _startRadius = 500f;
    private float _minRadius = 50f;

    public void Setup(float targetTime, Color color, float startRadius)
    {
        _rect = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _targetTime = targetTime;
        _image.color = color;
        _startRadius = startRadius;
    }

    public void UpdatePosition(float currentTime, float lookAheadTime)
    {
        float diff = _targetTime - currentTime;
        float progress = Mathf.Clamp01(diff / lookAheadTime); 

        float currentRadius = Mathf.Lerp(_minRadius, _startRadius, progress);
        _rect.sizeDelta = new Vector2(currentRadius * 2, currentRadius * 2);

        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f - progress);
    }
}
