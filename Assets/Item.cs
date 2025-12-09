using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // C# 기초: 'enum' (열거형) - 아이템 종류를 구분할 '꼬리표'
    public enum ItemType
    {
        EnergyDrink,  // 0
        CheatSheet,   // 1
        BrokenKeyboard // 2
    }

    // 유니티 인스펙터에서 이 아이템이 무슨 아이템인지 선택할 수 있습니다.
    public ItemType type;

    // (에너지 드링크용)
    public float speedBoostDuration = 5.0f; // 5초간
    public float speedMultiplier = 1.5f;    // 1.5배
    public float attackspeedup = 5.0f;
    // (부러진 키보드용)
    public float freezeDuration = 4.0f; // 4초간 멈춤

    // 'Is Trigger'가 체크된 콜라이더에 다른게 '들어왔을 때'
    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 들어온 녀석이 "Player" 태그가 맞는지?
        if (other.CompareTag("Player"))
        {
            // 2. 플레이어의 PlayerController 스크립트를 가져옴
            PlayerController player = other.GetComponent<PlayerController>();

            // C# 기초: switch (경우에 따라)
            // 인스펙터에서 설정한 'type'에 따라 다른 효과를 줍니다.
            switch (type)
            {
                case ItemType.EnergyDrink:
                    // '에너지 드링크'라면 플레이어의 SpeedUp 함수 호출
                    player.SpeedUp(speedBoostDuration, speedMultiplier);
                    player.AttackSpeedUp(speedBoostDuration, attackspeedup); // 공속 버프 추가
                    break;

                case ItemType.CheatSheet:
                    // '컨닝 페이퍼'라면 플레이어의 AddProjectile 함수 호출
                    player.AddProjectile();
                    break;

                case ItemType.BrokenKeyboard:
                    // '부러진 키보드'라면 GameManager의 함수를 호출 (광역 효과)
                    // (GameManager를 'Singleton'으로 만들면 편하지만, 일단 Find로)
                    GameManager.Instance.FreezeEnemies(freezeDuration);
                    break;
            }

            // 3. 아이템을 먹었으니, 아이템은 파괴
            Destroy(gameObject);
        }
    }
}