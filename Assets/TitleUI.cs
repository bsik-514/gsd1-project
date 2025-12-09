using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // 버튼 제어하려면 필요

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button startButton; // 버튼 연결 슬롯

    void Awake()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(() => SceneManager.LoadScene("SampleScene"));
        }
    }
}
