using UnityEditor.EditorTools;
using UnityEngine;

public class Player : MonoSingleton<Player>
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [SerializeField]
    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Vector2 _lookDirection = Vector2.right;
    public float baseDamage = 10f;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        PlayerManager.Instance.RegisterPlayer(this.transform, this);
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); 
        float moveY = Input.GetAxisRaw("Vertical");   
        _moveInput = new Vector2(moveX, moveY).normalized;

        if (_moveInput != Vector2.zero)
        {
            _lookDirection = _moveInput;
            spriteRenderer.flipX = _moveInput.x < 0;
        }
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void ProcessAttack(BeatRating rating)
    {
        if (rating == BeatRating.Miss)
        {
            Debug.Log("<color=red>Miss! 공격 실패</color>");
            return;
        }

        ExecuteAttack(rating);
    }

    private void ExecuteAttack(BeatRating rating)
    {
        float multiplier = (rating == BeatRating.Perfect) ? 2.0f : 1.0f;
        float finalDamage = baseDamage * multiplier;

        // 공격 이펙트 소환
        //GameObject effect = PoolingManager.Instance.Get("AttackEffect", transform.position, Quaternion.identity);

        CameraManager.Instance.Shake(rating == BeatRating.Perfect ? 1.5f : 0.5f, 0.1f);

        Debug.Log($"<color=cyan>{rating} Attack! Damage: {finalDamage}</color>");
    }
}
