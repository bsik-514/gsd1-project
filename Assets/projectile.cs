using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10.0f; // 탄환 속도
    public int damage = 1;      // 데미지
    public float rangeTime = 1.5f; // [추가] 사거리 (1.5초 동안만 날아감)

    private Transform target;

    void Start()
    {
        // [핵심] 태어나자마자 '시한부 선고' (설정된 시간 뒤에 자동 파괴)
        Destroy(gameObject, rangeTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        // 타겟이 있으면 유도, 없으면 그냥 직진
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            // 타겟이 사라지면(죽었으면) 그냥 바라보던 방향으로 직진하다가 사라짐
            // (혹은 바로 Destroy(gameObject) 해도 됩니다)
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 1. 컴파일 에러
            var enemy1 = other.GetComponent<Enemy_CompileError>();
            if (enemy1 != null) { enemy1.TakeDamage(damage); Destroy(gameObject); return; }

            // 2. 프로토타입
            var enemy2 = other.GetComponent<Enemy_Prototype>();
            if (enemy2 != null) { enemy2.TakeDamage(damage); Destroy(gameObject); return; }

            // 3. 보스
            var enemy3 = other.GetComponent<Enemy_Boss>();
            if (enemy3 != null) { enemy3.TakeDamage(damage); Destroy(gameObject); return; }

            // 스크립트 없어도 일단 적이면 파괴
            Destroy(gameObject);
        }
    }
}