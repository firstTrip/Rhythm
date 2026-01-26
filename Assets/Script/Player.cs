using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [SerializeField]
    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Vector2 _lookDirection = Vector2.right;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // 매니저에 자신을 등록 (이제 아무도 이 player를 찾으러 다닐 필요 없음)
        PlayerManager.Instance.RegisterPlayer(this.transform, this);
    }

    private void Update()
    {
        // 1. WASD 입력 받기
        float moveX = Input.GetAxisRaw("Horizontal"); // A(-1), D(1)
        float moveY = Input.GetAxisRaw("Vertical");   // W(1), S(-1)
        _moveInput = new Vector2(moveX, moveY).normalized;

        // 이동 방향 저장 (공격 방향 결정용)
        if (_moveInput != Vector2.zero)
        {
            _lookDirection = _moveInput;
            // 캐릭터 좌우 반전
            spriteRenderer.flipX = _moveInput.x < 0;
        }

        // 2. 공격 입력 확인 (Space 또는 Enter)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            PerformAttack();
        }
    }

    private void FixedUpdate()
    {
        // 물리 기반 이동 (떨림 방지)
        _rb.MovePosition(_rb.position + _moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void PerformAttack()
    {
        Debug.Log("공격 실행!");
    }
}
