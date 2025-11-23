using UnityEngine;
using System.Collections;

public class Enemy_Prototype : MonoBehaviour
{
    [Header("스탯 설정")]
    public float moveSpeed = 1.0f; // 느림
    public int hp = 10; // 체력 많음
    public float stopDistance = 5.0f; // 플레이어와 이 정도 거리 유지

    [Header("공격 설정")]
    public GameObject bulletPrefab; // 아까 만든 EnemyBullet 프리팹 연결
    public float fireRate = 2.0f;   // 2초마다 발사
    private float fireTimer = 0f;

    private Transform playerTarget;
    private SpriteRenderer sp; // 피격 효과용

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
    }

    void Update()
    {
        if (playerTarget == null) return;

        // 1. 플레이어와의 거리 계산
        float distance = Vector2.Distance(transform.position, playerTarget.position);

        // 2. 방향 계산
        Vector2 direction = (playerTarget.position - transform.position).normalized;

        // 3. 거리가 멀면 다가가고, 가까우면 멈춤 (원거리 딜러니까)
        if (distance > stopDistance)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }

        // 4. 공격 (타이머)
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0;
            Shoot(direction);
        }
    }

    void Shoot(Vector2 dir)
    {
        // 총알 생성
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        // 총알에게 날아갈 방향 알려줌
        bullet.GetComponent<EnemyProjectile>().SetDirection(dir);
    }

    // --- 데미지 받는 기능 (CompileError랑 똑같음) ---
    public void TakeDamage(int damage)
    {
        hp -= damage;
        StartCoroutine(FlashRed());
        if (hp <= 0) Destroy(gameObject);
    }

    IEnumerator FlashRed()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sp.color = Color.white;
    }

    // 얼리기 기능도 필요하면 추가...
}