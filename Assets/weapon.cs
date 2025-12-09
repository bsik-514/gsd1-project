using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("무기 설정")]
    public GameObject projectilePrefab; // 발사체 프리팹
    public float fireRate = 1.4f;       // 발사 속도
    public int projectileCount = 1;     // 발사체 개수
    public float attackRange = 6.0f;    // [추가] 공격 인식 범위 (이 안의 적만 공격)

    [Header("사운드")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    private float fireTimer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Fire();
        }
    }

    void Fire()
    {
        // 사거리 안의 적 찾기
        Transform closestEnemy = FindClosestEnemy();

        // 적이 존재하면 (사거리 안에 적이 없으면 null이 됨)
        if (closestEnemy != null)
        {
            // 소리 재생
            if (shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            // 발사
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
                Vector3 spawnPos = transform.position + randomOffset;

                GameObject projectileGO = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                Projectile projectileScript = projectileGO.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.SetTarget(closestEnemy);
                }
            }
        }
    }

    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, currentPosition);

            // [수정된 부분]
            // 1. 현재까지 찾은 적보다 더 가깝고
            // 2. 내 공격 범위(attackRange) 안에 있어야 함
            if (distance < minDistance && distance <= attackRange)
            {
                minDistance = distance;
                closest = enemy.transform;
            }
        }
        return closest;
    }

    public void AddProjectile()
    {
        projectileCount++;
        Debug.Log("코딩 파편 탄수 증가!");
    }

    // [보너스] 유니티 에디터에서 공격 범위를 눈으로 보여주는 기능
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // 빨간색 선
        Gizmos.DrawWireSphere(transform.position, attackRange); // 공격 범위만큼 원 그리기
    }
}