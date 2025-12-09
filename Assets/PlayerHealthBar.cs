using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public PlayerController player;
    public Image fillImage;
    private int maxHP;

    void Start()
    {
        if (player == null)
            player = GetComponentInParent<PlayerController>(); // 부모에서 찾기

        maxHP = player.life;
    }

    void Update()
    {
        // 체력 비율 계산
        float ratio = (float)player.life / maxHP;
        fillImage.fillAmount = Mathf.Clamp01(ratio);

        // 항상 카메라를 바라보게
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0); // 뒤집힘 방지
    }
}
