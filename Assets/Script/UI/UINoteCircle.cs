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

        float alpha = diff > 0 ? 1f - progress : 1f + (diff * 5f);
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.Clamp01(alpha));

        if (diff < -0.15f)
        {
            OnMiss();
        }
    }

    private void OnMiss()
    {
        PoolingManager.Instance.Release(this.gameObject);
    }
}
