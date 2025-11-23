using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("기본 설정")]
    public int currentFloor = 4; // 현재 층 (4층부터 시작)
    public Vector2 spawnAreaMin = new Vector2(-10, -10);
    public Vector2 spawnAreaMax = new Vector2(10, 10);

    [Header("몬스터 프리팹")]
    public GameObject compileErrorPrefab;   // 컴파일 에러
    public GameObject reportBugPrefab;      // 리포트 버그
    public GameObject prototypePrefab;      // 프로토타입
    public GameObject professorBossPrefab;  // [추가됨] 교수님 보스

    [Header("아이템 프리팹")]
    public GameObject energyDrinkPrefab;
    public GameObject cheatSheetPrefab;
    public GameObject brokenKeyboardPrefab;

    [Header("UI 및 타이머")]
    public float stageTimeLimit = 60.0f; // 1분 버티기
    public Text timerText;

    private float currentTimer;
    private bool isTimerRunning = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartStage(4); // 게임 시작 시 4층부터
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (currentTimer > 0)
            {
                currentTimer -= Time.deltaTime;
                if (timerText != null) UpdateTimerUI(currentTimer);
            }
            else
            {
                // 시간 종료! (스테이지 클리어)
                currentTimer = 0;
                isTimerRunning = false;
                if (timerText != null) UpdateTimerUI(0);

                StartCoroutine(StageClearProcess());
            }
        }
    }

    // --- 스테이지 시작 함수 ---
    void StartStage(int floor)
    {
        currentFloor = floor;
        Debug.Log(currentFloor + "층 시작!");

        // 1. 타이머 리셋
        currentTimer = stageTimeLimit;
        isTimerRunning = true;

        // 2. 층별 몬스터/아이템 스폰 로직
        if (floor == 4)
        {
            // [4층] 컴파일 에러 떼거지
            SpawnEnemy(compileErrorPrefab, 30);

            // 아이템 소량 지원
            SpawnItem(energyDrinkPrefab, 1);
        }
        else if (floor == 3)
        {
            // [3층] 컴파일 에러 + 리포트 버그 + 프로토타입(소수)
            SpawnEnemy(compileErrorPrefab, 20);
            SpawnEnemy(reportBugPrefab, 15);
            SpawnEnemy(prototypePrefab, 3);

            // 아이템 지원
            SpawnItem(energyDrinkPrefab, 2);
            SpawnItem(cheatSheetPrefab, 1);
            SpawnItem(brokenKeyboardPrefab, 1);
        }
        else if (floor == 2)
        {
            // [2층] 교수님 등장! (보스전)
            Debug.Log("교수님 출현!");

            // 교수님 1명 스폰
            SpawnEnemy(professorBossPrefab, 1);

            // 부하들도 같이 나옴
            SpawnEnemy(reportBugPrefab, 10);
            SpawnEnemy(prototypePrefab, 5);

            // 보스전이라 아이템 많이 줌
            SpawnItem(energyDrinkPrefab, 2);
            SpawnItem(cheatSheetPrefab, 2);
            SpawnItem(brokenKeyboardPrefab, 1);
        }
    }

    // --- 스테이지 클리어 처리 (엘리베이터) ---
    IEnumerator StageClearProcess()
    {
        Debug.Log("엘리베이터 탑승 중... (잠시 대기)");

        // 1. 화면 청소
        ClearAllObjects();

        // 2. 3초 대기
        yield return new WaitForSeconds(3.0f);

        // 3. 다음 층 이동
        int nextFloor = currentFloor - 1;
        if (nextFloor >= 1) // 1층까지만
        {
            StartStage(nextFloor);
        }
        else
        {
            Debug.Log("축하합니다! 공학관 탈출 성공!");
            Time.timeScale = 0f; // 게임 종료
        }
    }

    // --- 유틸리티 함수들 ---

    void SpawnEnemy(GameObject prefab, int count)
    {
        if (prefab == null) return;
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = GetRandomPosition();
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    void SpawnItem(GameObject prefab, int count)
    {
        if (prefab == null) return;
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = GetRandomPosition();
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    void ClearAllObjects()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in enemies) Destroy(obj);

        // (아이템도 지우려면 태그 설정 필요)
    }

    Vector2 GetRandomPosition()
    {
        Vector2 spawnPos = Vector2.zero;
        bool isSafe = false;
        int attempts = 0;

        while (!isSafe && attempts < 100)
        {
            float spawnX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float spawnY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            spawnPos = new Vector2(spawnX, spawnY);

            // 플레이어(0,0)와 5미터 이상 거리두기
            if (Vector2.Distance(spawnPos, Vector2.zero) > 5.0f) isSafe = true;
            attempts++;
        }
        return spawnPos;
    }

    public void FreezeEnemies(float duration)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            // 몬스터 종류가 다양해졌으니, 각 스크립트를 확인해서 Freeze 호출

            // 1. 컴파일 에러
            var script1 = enemy.GetComponent<Enemy_CompileError>();
            if (script1 != null) script1.Freeze(duration);

            // 2. (추가) 프로토타입이나 보스 등 다른 적에게도 Freeze가 있다면 호출
            // (일단은 컴파일 에러만 멈추도록 둠, 필요하면 아래 주석 해제)
            // var script2 = enemy.GetComponent<Enemy_Boss>();
            // if (script2 != null) script2.Freeze(duration);
        }
    }

    void UpdateTimerUI(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}