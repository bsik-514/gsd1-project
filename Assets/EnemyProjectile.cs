using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 7.0f;
    public int damage = 1;

    private Vector3 direction;

    // 몬스터가 이 함수를 호출해서 방향을 알려줌
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        // 알려준 방향으로 계속 직진
        transform.Translate(direction * speed * Time.deltaTime);

        // 화면 밖으로 너무 멀리 가면 삭제 (성능 관리)
        if (transform.position.magnitude > 20.0f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어랑 부딪히면?
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage); // 플레이어 아야!
            }
            Destroy(gameObject); // 총알 소멸
        }
    }
}