using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public int life = 3;

    private Rigidbody2D rb;
    private Weapon weapon;
    private SpriteRenderer spriteRenderer; // 깜빡거림 효과를 위해 필요

    private bool isInvincible = false; // 지금 무적 상태인가?

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponent<Weapon>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // 컴포넌트 가져오기
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    // --- 공격받는 함수 수정 ---
    public void TakeDamage(int damageAmount)
    {
        // 무적 상태면 데미지 무시!
        if (isInvincible) return;

        life -= damageAmount;
        Debug.Log("공돌이 라이프: " + life);

        if (life <= 0)
        {
            Die();
        }
        else
        {
            // 안 죽었으면 무적 시간 발동
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    // 무적 코루틴
    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true; // 무적 켜기

        // 시각적 효과: 빨간색으로 변함
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(1.0f); // 1초 동안 대기

        // 원래 색으로 복구
        spriteRenderer.color = Color.white;
        isInvincible = false; // 무적 끄기
    }

    void Die()
    {
        Debug.Log("GAME OVER");
        Destroy(gameObject);
        // 여기에 나중에 '게임오버 UI' 띄우는 코드 추가
    }

    // (아이템 효과 함수들은 그대로 둡니다)
    public void SpeedUp(float duration, float speedMultiplier)
    {
        StartCoroutine(SpeedUpCoroutine(duration, speedMultiplier));
    }

    private IEnumerator SpeedUpCoroutine(float duration, float speedMultiplier)
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= speedMultiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalSpeed;
    }

    public void AddProjectile()
    {
        if (weapon != null) weapon.AddProjectile();
    }
}