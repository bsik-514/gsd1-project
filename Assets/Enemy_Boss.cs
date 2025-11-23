using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : MonoBehaviour
{
    [Header("보스 스탯")]
    public float moveSpeed = 0.8f; // 중후하게 천천히 움직임
    public int hp = 50;            // 압도적인 체력
    public int scoreValue = 1000;  // 처치 시 점수 (나중을 위해)

    [Header("공격 설정")]
    public GameObject projectilePrefab; // 시험지 프리팹
    public float fireRate = 2.0f;       // 공격 주기
    public int bulletCount = 5;         // 한 번에 발사할 시험지 개수
    public float spreadAngle = 60.0f;   // 부채꼴 각도

    private Transform playerTarget;
    private float fireTimer = 0f;
    private SpriteRenderer sp;

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
    }

    void Update()
    {
        if (playerTarget == null) return;

        // 1. 플레이어를 향해 천천히 이동
        Vector2 direction = (playerTarget.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // 2. 공격 타이머
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0;
            ShotgunFire(direction); // 샷건 발사!
        }
    }

    // 부채꼴 발사 함수 (수학이 좀 들어감)
    void ShotgunFire(Vector2 direction)
    {
        // 중심 각도 계산 (아크탄젠트 사용)
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 시작 각도 (부채꼴의 맨 왼쪽)
        float startAngle = baseAngle - spreadAngle / 2f;

        // 총알 사이의 각도 간격
        float angleStep = spreadAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            // 현재 총알의 각도 계산
            float currentAngle = startAngle + (angleStep * i);

            // 각도를 벡터(방향)로 변환
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector3 bulletDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);

            // 총알 생성 및 발사
            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<EnemyProjectile>().SetDirection(bulletDir);
        }
    }

    // 데미지 처리
    public void TakeDamage(int damage)
    {
        hp -= damage;
        StartCoroutine(FlashEffect());

        if (hp <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashEffect()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sp.color = Color.white;
    }

    void Die()
    {
        // 보스 죽음 처리 (나중에 클리어 UI 띄우기 연결 가능)
        Debug.Log("교수님 처치 완료! A+ 획득!");
        Destroy(gameObject);

        // 게임 매니저에게 "보스 죽었음" 알리는 기능 추가 가능
    }

    // 얼리기 기능 (필요하면 추가)
    public void Freeze(float duration) { /* ... */ }
}