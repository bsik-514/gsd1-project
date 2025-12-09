using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public int life = 10;

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

        if (moveX < 0)
            spriteRenderer.flipX = true;   // 왼쪽 바라보기
        else if (moveX > 0)
            spriteRenderer.flipX = false;  // 오른쪽 바라보기

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

    public GameOverUI gameOverUI;  // Inspector에서 GameOverCanvas 드래그

    void Die()
    {
        Debug.Log("GAME OVER");

        // 움직임/공격 정지
        enabled = false;
        if (weapon) weapon.enabled = false;
        rb.linearVelocity = Vector2.zero;

        // UI 표시 (파괴 X)
        if (gameOverUI) gameOverUI.Show();
    
    }
    // 여기에 나중에 '게임오버 UI' 띄우는 코드 추가


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

    public void AttackSpeedUp(float duration, float multiplier)
    {
        StartCoroutine(AttackSpeedUpCoroutine(duration, multiplier));
    }

    private IEnumerator AttackSpeedUpCoroutine(float duration, float multiplier)
    {
        if (weapon == null) yield break;

        float originalFireRate = weapon.fireRate;      // fireRate는 보통 공격 주기 (작을수록 빠름)
        weapon.fireRate /= multiplier;                 // multiplier=5면 5배 빨라짐

        yield return new WaitForSeconds(duration);

        weapon.fireRate = originalFireRate;            // 원래 속도로 복구
    }

    public void AddProjectile()
    {
        if (weapon != null) weapon.AddProjectile();
    }
}