using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    public Transform target; // 추적할 플레이어의 Transform
    public Vector3 offset;   // 위치 보정값 (캐릭터 중심에 맞추기 위함)

    private RectTransform _rectTransform;
    private Camera _mainCam;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _mainCam = Camera.main;

        if (target == null)
        {
            target =PlayerManager.Instance.PlayerTransform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 screenPos = _mainCam.WorldToScreenPoint(target.position + offset);
        if (screenPos.z < 0) return;

        _rectTransform.position = screenPos;
    }
}
