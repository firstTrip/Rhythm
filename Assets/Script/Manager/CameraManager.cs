using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    [SerializeField] private CinemachineCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin _noise;
    private float _defaultZoom;
    private Coroutine _zoomCoroutine;

    protected override void Awake()
    {
        base.Awake();

        if (virtualCamera != null)
        {
            _noise = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            _defaultZoom = virtualCamera.Lens.OrthographicSize;
        }
    }

    public void ZoomTo(float targetSize, float duration)
    {
        if (_zoomCoroutine != null) StopCoroutine(_zoomCoroutine);
        _zoomCoroutine = StartCoroutine(AnimateZoom(targetSize, duration));
    }

    private IEnumerator AnimateZoom(float targetSize, float duration)
    {
        float startSize = virtualCamera.Lens.OrthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            virtualCamera.Lens.OrthographicSize = Mathf.SmoothStep(startSize, targetSize, elapsed / duration);
            yield return null;
        }
        virtualCamera.Lens.OrthographicSize = targetSize;
    }

    public void Shake(float intensity = 1f, float time = 0.2f)
    {
        if (_noise == null)
        {
            Debug.LogWarning("CinemachineBasicMultiChannelPerlin 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        StartCoroutine(StartShake(intensity, time));
    }

    private IEnumerator StartShake(float intensity, float time)
    {
        _noise.AmplitudeGain = intensity;
        yield return new WaitForSeconds(time);
        _noise.AmplitudeGain = 0f;
    }
}
