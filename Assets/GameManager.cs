using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("기본 설정")]
    public int currentFloor = 5; // 5층(1학년) 시작
    public Vector2 spawnAreaMin = new Vector2(-10, -10);
    public Vector2 spawnAreaMax = new Vector2(10, 10);

    [Header("몬스터 프리팹")]
    public GameObject weakEnemyPrefab;    // 1학년/군대 잡몹
    public GameObject normalEnemyPrefab;  // 2,3학년 잡몹
    public GameObject rangedEnemyPrefab;  // 원거리 잡몹

    [Header("보스 프리팹")]
    public GameObject bossJava;       // 1학년
    public GameObject bossArmy;       // 4층
    public GameObject bossAlgo;       // 2학년
    public GameObject bossOS;         // 3학년
    public GameObject bossGrad;       // 4학년

    [Header("아이템 프리팹")]
    public GameObject energyDrinkPrefab;
    public GameObject cheatSheetPrefab;
    public GameObject brokenKeyboardPrefab;

    [Header("UI 및 연출")]
    public Text timerText;
    public Text floorText;          // [추가] 층수 표시 텍스트
    public SpriteRenderer mapRenderer; // [추가] 배경 색 바꿀 맵 이미지
    public RectTransform leftDoor;
    public RectTransform rightDoor;

    // 내부 변수
    private float currentTimer;
    private bool isTimerRunning = false;
    private bool isBossSpawned = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 배경 맵 자동 찾기 (연결 안 했을 경우 대비)
        if (mapRenderer == null)
        {
            GameObject mapObj = GameObject.Find("Map_Background");
            if (mapObj != null) mapRenderer = mapObj.GetComponent<SpriteRenderer>();
        }

        StartCoroutine(StartGameTransition());
    }

    IEnumerator StartGameTransition()
    {
        leftDoor.anchoredPosition = new Vector2(0, 0);
        rightDoor.anchoredPosition = new Vector2(0, 0);
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(AnimateDoors(false));
        StartStage(5);
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (currentTimer > 0)
            {
                // [군대 시간] 4층은 시간이 0.5배속으로 흐름
                float timeMultiplier = (currentFloor == 4) ? 0.5f : 1.0f;
                currentTimer -= Time.deltaTime * timeMultiplier;

                UpdateTimerUI(currentTimer);

                // [룰] 60초 남았을 때 보스 등장
                if (currentTimer <= 60.0f && !isBossSpawned)
                {
                    SpawnBossByFloor(currentFloor);
                    isBossSpawned = true;
                }
            }
            else
            {
                currentTimer = 0;
                isTimerRunning = false;
                StartCoroutine(NextStageProcess());
            }
        }
    }

    void StartStage(int floor)
    {
        currentFloor = floor;
        isBossSpawned = false;
        currentTimer = 120.0f;
        isTimerRunning = true;

        // [추가] 층수 UI 갱신
        if (floorText != null) floorText.text = floor + "F";

        Debug.Log(floor + "층 시작! (" + GetFloorTheme(floor) + ")");

        // [추가] 층별 배경 색깔 변경 (분위기 전환)
        if (mapRenderer != null)
        {
            switch (floor)
            {
                case 5: mapRenderer.color = Color.white; break; // 기본
                case 4: mapRenderer.color = new Color(0.6f, 0.7f, 0.6f); break; // 국방색 (군대)
                case 3: mapRenderer.color = new Color(0.7f, 0.7f, 0.9f); break; // 푸르스름 (2학년)
                case 2: mapRenderer.color = new Color(0.9f, 0.7f, 0.7f); break; // 붉은끼 (3학년)
                case 1: mapRenderer.color = new Color(0.5f, 0.5f, 0.5f); break; // 어두침침 (4학년)
            }
        }

        // --- 층별 몬스터 스폰 ---
        if (floor == 5)
        {
            SpawnEnemy(weakEnemyPrefab, 20);
            SpawnItem(energyDrinkPrefab, 1);
        }
        else if (floor == 4)
        {
            SpawnEnemy(weakEnemyPrefab, 15);
            SpawnEnemy(normalEnemyPrefab, 15);
            SpawnItem(cheatSheetPrefab, 1);
        }
        else if (floor == 3)
        {
            SpawnEnemy(normalEnemyPrefab, 20);
            SpawnEnemy(rangedEnemyPrefab, 5);
            SpawnItem(energyDrinkPrefab, 2);
        }
        else if (floor == 2)
        {
            SpawnEnemy(normalEnemyPrefab, 20);
            SpawnEnemy(rangedEnemyPrefab, 10);
            SpawnItem(brokenKeyboardPrefab, 1);
        }
        else if (floor == 1)
        {
            SpawnEnemy(weakEnemyPrefab, 20);
            SpawnEnemy(normalEnemyPrefab, 20);
            SpawnEnemy(rangedEnemyPrefab, 20);
            SpawnItem(energyDrinkPrefab, 3);
            SpawnItem(cheatSheetPrefab, 2);
            SpawnItem(brokenKeyboardPrefab, 2);
        }
    }

    void SpawnBossByFloor(int floor)
    {
        Debug.Log("⚠️ 보스 출현!");
        GameObject bossToSpawn = null;

        switch (floor)
        {
            case 5: bossToSpawn = bossJava; break;
            case 4: bossToSpawn = bossArmy; break;
            case 3: bossToSpawn = bossAlgo; break;
            case 2: bossToSpawn = bossOS; break;
            case 1: bossToSpawn = bossGrad; break;
        }

        if (bossToSpawn != null) SpawnEnemy(bossToSpawn, 1);
        SpawnEnemy(weakEnemyPrefab, 5);
    }

    IEnumerator NextStageProcess()
    {
        Debug.Log("문이 닫힙니다.");
        yield return StartCoroutine(AnimateDoors(true));

        ClearAllObjects();
        yield return new WaitForSeconds(1.0f);

        int nextFloor = currentFloor - 1;
        if (nextFloor >= 1)
        {
            StartStage(nextFloor);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) player.transform.position = Vector3.zero;
            yield return StartCoroutine(AnimateDoors(false));
        }
        else
        {
            Debug.Log("졸업 축하합니다!");
            Time.timeScale = 0f;
        }
    }

    IEnumerator AnimateDoors(bool isClosing)
    {
        float timer = 0f;
        float duration = 1.0f;
        float leftTargetX = isClosing ? 0f : -leftDoor.rect.width;
        float rightTargetX = isClosing ? 0f : rightDoor.rect.width;

        Vector2 leftStart = leftDoor.anchoredPosition;
        Vector2 rightStart = rightDoor.anchoredPosition;
        Vector2 leftEnd = new Vector2(leftTargetX, leftStart.y);
        Vector2 rightEnd = new Vector2(rightTargetX, rightStart.y);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            leftDoor.anchoredPosition = Vector2.Lerp(leftStart, leftEnd, t);
            rightDoor.anchoredPosition = Vector2.Lerp(rightStart, rightEnd, t);
            yield return null;
        }
        leftDoor.anchoredPosition = leftEnd;
        rightDoor.anchoredPosition = rightEnd;
    }

    string GetFloorTheme(int floor)
    {
        switch (floor)
        {
            case 5: return "1학년";
            case 4: return "군대";
            case 3: return "2학년";
            case 2: return "3학년";
            case 1: return "4학년";
            default: return "";
        }
    }

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
    }

    Vector2 GetRandomPosition()
    {
        Vector2 spawnPos = Vector2.zero;
        bool isSafe = false;
        int attempts = 0;
        while (!isSafe && attempts < 100)
        {
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            spawnPos = new Vector2(x, y);
            if (Vector2.Distance(spawnPos, Vector2.zero) > 5.0f) isSafe = true;
            attempts++;
        }
        return spawnPos;
    }

    void UpdateTimerUI(float time)
    {
        if (time < 0) time = 0;
        float m = Mathf.FloorToInt(time / 60);
        float s = Mathf.FloorToInt(time % 60);
        if (timerText != null) timerText.text = string.Format("{0:00}:{1:00}", m, s);
    }

    public void FreezeEnemies(float duration)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            var script = enemy.GetComponent<Enemy_CompileError>();
            if (script != null) script.Freeze(duration);
            var script2 = enemy.GetComponent<Enemy_Boss>();
            if (script2 != null) script2.Freeze(duration);
        }
    }
}