using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Image panelDim;        // 검은 배경
    public Image gameOverImage;   // PNG 이미지
    public float dimDuration = 0.4f;
    public float imageFadeDelay = 0.1f;
    public float imageFadeDuration = 0.35f;

    void Awake()
    {
        // 안전 초기화
        SetAlpha(panelDim, 0f);
        SetAlpha(gameOverImage, 0f);
        gameObject.SetActive(false); // 처음엔 꺼둠
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(Co_Show());
    }

    IEnumerator Co_Show()
    {
        Time.timeScale = 0f; // 게임 정지 (UI는 동작)

        // 화면 어둡게
        yield return Fade(panelDim, 0f, 0.85f, dimDuration);

        // 약간 쉬고 메인 이미지 등장
        yield return new WaitForSecondsRealtime(imageFadeDelay);
        yield return Fade(gameOverImage, 0f, 1f, imageFadeDuration);
    }

    public void OnClickRetry()
    {
        Time.timeScale = 1f;                 // 일시정지 풀기
        SceneManager.LoadScene("titlescene"); // ← 메인(타이틀) 씬 이름
    }

    static void SetAlpha(Graphic g, float a)
    {
        if (!g) return;
        var c = g.color; c.a = a; g.color = c;
    }
    IEnumerator Fade(Graphic g, float from, float to, float duration)
    {
        if (!g) yield break;
        float t = 0f;
        SetAlpha(g, from);
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // 정지 중에도 진행
            SetAlpha(g, Mathf.Lerp(from, to, t / duration));
            yield return null;
        }
        SetAlpha(g, to);
    }
}
