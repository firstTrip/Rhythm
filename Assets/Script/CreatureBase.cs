using UnityEditor.EditorTools;
using UnityEngine;

public abstract class CreatureBase : MonoBehaviour
{
    protected MonsterData _data;

    // 변하는 실시간 데이터들
    protected float _health;
    protected float _moveSpeed;
    protected float _damage;


    protected Transform _player;
    protected bool _isDead = false;

    public virtual void Init(MonsterData data, Transform player, float elapsedTime)
    {
        this._data = data; // 주입받은 데이터 저장
        this._player = player;

        // 주입받은 SO의 기본 스탯 + 시간 기반 보정 적용
        CalculateFinalStats(elapsedTime);

        _isDead = false;
        gameObject.SetActive(true);
    }

    protected virtual void CalculateFinalStats(float elapsedTime)
    {
        // 1분당 난이도 보정치는 StageManager에서 가져옴
        float minutes = elapsedTime / 60f;
        float hMult = StageManager.Instance.currentStageData.healthMultiplierPerMin;

        // 최종 스탯 확정
        this._health = _data.maxHealth * Mathf.Pow(hMult, minutes);
        this._moveSpeed = _data.moveSpeed;
        this._damage = _data.damage;
    }

    protected virtual void Update()
    {
        if (_isDead || _player == null) return;
        Move();
    }

    protected virtual void Move()
    {
        if (_player == null) return;

        // 1. 플레이어를 향한 기본 방향
        Vector3 targetPos = PlayerManager.Instance.CachedPlayerPos;
        Vector3 moveDir = (targetPos - transform.position).normalized;

        // 2. 주변 몬스터 감지 및 분리(Separation) 계산
        Vector3 separationDir = Vector3.zero;

        // 성능을 위해 OverlapCircle 대신 가벼운 Distance 체크나 
        // 특정 레이어(Monster)만 골라내는 방식을 사용합니다.
        float detectionRadius = 0.5f; // 몬스터끼리 유지하고 싶은 최소 거리
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, detectionRadius, LayerMask.GetMask("Monster"));

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject) continue;

            // 나에게서 멀어지는 방향 계산
            Vector3 diff = transform.position - neighbor.transform.position;

            // 거리가 가까울수록 더 강하게 밀어냄 (역자승 법칙)
            float distance = diff.magnitude;
            if (distance < detectionRadius)
            {
                separationDir += diff.normalized / (distance + 0.1f);
            }
        }

        // 3. 최종 방향 = 플레이어 방향 + 분리 방향 * 가중치
        // 분리 가중치(1.5f)를 조절하여 얼마나 서로를 강하게 밀어낼지 결정합니다.
        Vector3 finalDir = (moveDir + separationDir * 1.5f).normalized;

        // 4. 이동 적용
        transform.position += finalDir * _moveSpeed * Time.deltaTime;

        // 5. 방향 전환 (스프라이트 반전)
        if (finalDir.x != 0)
            transform.localScale = new Vector3(finalDir.x > 0 ? -1 : 1, 1, 1);
    }

    public virtual void TakeDamage(float amount)
    {
        _health -= amount;
        if (_health <= 0) Die();
    }

    protected virtual void Die()
    {
        _isDead = true;
        // PoolManager로 반환 (이전 가이드 참고)
        PoolingManager.Instance.Release(gameObject);
    }
}
