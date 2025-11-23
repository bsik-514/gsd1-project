using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_CompileError : MonoBehaviour
{
    public float moveSpeed = 1.5f; // 속도 조금 줄임 (밸런스)
    public int damage = 1;
    public int hp = 3; // <-- 체력 추가! (3대 맞으면 죽음)

    private Rigidbody2D rb;
    private Transform playerTarget;
    private bool isFrozen = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (playerTarget != null && !isFrozen)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null) player.TakeDamage(damage);
        }
    }

    // --- [새로 추가된 부분] 데미지 받는 함수 ---
    public void TakeDamage(int amount)
    {
        hp -= amount;

        // 피격 효과: 잠깐 빨개짐
        StartCoroutine(FlashRed());

        if (hp <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void Die()
    {
        // 죽음 이펙트나 소리가 있다면 여기서 재생
        Destroy(gameObject); // 적 삭제
    }

    public void Freeze(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        GetComponent<SpriteRenderer>().color = Color.blue;
        yield return new WaitForSeconds(duration);
        GetComponent<SpriteRenderer>().color = Color.white;
        isFrozen = false;
    }
}