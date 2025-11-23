using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject projectilePrefab; // 발사체 프리팹
    public float fireRate = 0.7f; // 발사 주기 (아까 처방전대로 0.7초)
    public int projectileCount = 1; // 발사체 개수

    // [오류 해결] 소리 관련 변수 추가 (일단 선언만 해둠)
    public AudioClip shootSound;
    private AudioSource audioSource;

    private float fireTimer = 0f;

    void Start()
    {
        // Player에 AudioSource 컴포넌트가 있다면 가져옴
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
        Transform closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            // 소리가 연결되어 있다면 재생 (없으면 무시)
            if (shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            // 탄환 개수만큼 발사
            for (int i = 0; i < projectileCount; i++)
            {
                // [수정됨] 겹침 방지를 위해 위치를 살짝 랜덤하게 흩뿌림
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
            if (distance < minDistance)
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
        Debug.Log("코딩 파편 탄수 증가! 현재: " + projectileCount);
    }
}