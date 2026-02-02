using UnityEngine;
using UnityEngine.UI;

public class UINoteCircle : MonoBehaviour
{
    public float TargetTime => _targetTime;

    public SubBeatData Data { get; private set; }
    public BarData ParentBar { get; private set; }
    public bool IsHit { get; set; } = false;
    
    private bool _isGrayMode = false;
    private Image _image;

    private float _startRadius = 500f;
    private float _minRadius = 50f;
    private RectTransform _rect;

    private float _targetTime;
    public void Setup(SubBeatData data, BarData parentBar, Color color, float startRadius)
    {
        this.Data = data;
        this.ParentBar = parentBar;
        _rect = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _targetTime = Data.time;
        _image.color = color;
        _startRadius = startRadius;
    }

    public void SetColor(Color color)
    {
        _isGrayMode = true;
        if (_image == null) _image = GetComponent<Image>();

        _image.color = color;
    }

    public bool UpdatePosition(float currentTime, float lookAheadTime)
    {
        if (IsHit) return false;

        float diff = _targetTime - currentTime;

        if (diff < -0.15f)
        {
            ParentBar.isFailed = true;
            RhythmCircleVisualizer.Instance.SetBarColorGray(ParentBar);
            OnMiss();
            return false;
        }
        float progress = Mathf.Clamp01(diff / lookAheadTime);
        float currentRadius = Mathf.Lerp(_minRadius, _startRadius, progress);
        _rect.sizeDelta = new Vector2(currentRadius * 2, currentRadius * 2);

        if (!_isGrayMode)
        {
            float alpha = diff > 0 ? 1f - progress : 1f + (diff * 5f);
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.Clamp01(alpha));
        }
        else
        {
            float alpha = diff > 0 ? 1f - progress : 1f + (diff * 5f);
            _image.color = new Color(0.5f, 0.5f, 0.5f, Mathf.Clamp01(alpha));
        }

        return true;
    }

    private void OnMiss()
    {
        PoolingManager.Instance.Release(this.gameObject);
    }
}
