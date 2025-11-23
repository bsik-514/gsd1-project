using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10.0f;
    public int damage = 1;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 1. 컴파일 에러인가?
            Enemy_CompileError error = other.GetComponent<Enemy_CompileError>();
            if (error != null)
            {
                error.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            // 2. 프로토타입인가?
            Enemy_Prototype proto = other.GetComponent<Enemy_Prototype>();
            if (proto != null)
            {
                proto.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            // 3. 보스(교수님)인가? (미리 추가)
            Enemy_Boss boss = other.GetComponent<Enemy_Boss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            // 만약 Enemy 태그는 있는데 스크립트가 없는 경우 그냥 파괴
            Destroy(gameObject);
        }
    }
}