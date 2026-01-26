using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    public Transform PlayerTransform { get; private set; }
    public Player Controller { get; private set; }
    public Vector3 CachedPlayerPos { get; private set; }

    // 플레이어의 현재 레벨, 경험치 등 상태도 여기서 관리하면 좋습니다.
    public int CurrentLevel { get; private set; } = 1;

    public void RegisterPlayer(Transform player, Player controller)
    {
        PlayerTransform = player;
        Controller = controller;
    }

    private void Update()
    {
        if (PlayerTransform != null)
            CachedPlayerPos = PlayerTransform.position;
    }
}
