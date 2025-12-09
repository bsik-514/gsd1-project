using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("기본 설정")]
    public int currentFloor = 5; // 5층(1학년) 시작
    public Vector2 spawnAreaMin = new Vector2(-2560, -2560);
    public Vector2 spawnAreaMax = new Vector2(1280, 1280);

    [Header("몬스터 프리팹 (잡몹)")]
    public GameObject weakEnemyPrefab;    // 1학년/군대 잡몹
    public GameObject normalEnemyPrefab;  // 2,3학년 잡몹
    public GameObject rangedEnemyPrefab;  // 원거리 잡몹

    [Header("보스 프리팹 (과목)")]
    public GameObject bossJava;       // 1학년 보스 (자바)
    public GameObject bossArmy;       // 4층 보스 (전역증)
    public GameObject bossAlgo;       // 2학년 보스 (알고리즘)
    public GameObject bossOS;         // 3학년 보스 (운영체제)
    public GameObject bossGrad;       // 4학년 보스 (졸업작품)

    [Header("아이템 프리팹")]
    public GameObject energyDrinkPrefab;
    public GameObject cheatSheetPrefab;
    public GameObject brokenKeyboardPrefab;

    [Header("UI 및 연출")]
    public Text timerText;
    public Text floorText; // 벽에 붙은 층수 텍스트
    public RectTransform leftDoor;  // 왼쪽 문 UI
    public RectTransform rightDoor; // 오른쪽 문 UI

    // 내부 변수
    private float currentTimer;
    private bool isTimerRunning = false;
    private bool isBossSpawned = false;

    [Header("엔딩 연출")]
    public GameObject graduationImage;      // GraduationImage 오브젝트
    public CanvasGroup graduationCanvasGroup; // 위 오브젝트의 CanvasGroup

    public void BossDied()                   // 보스가 죽을 때 호출됨
    {
        if (currentFloor == 1)               // 1층(마지막 층)일 때만 엔딩
            StartCoroutine(ShowGraduationImage());
    }

    IEnumerator ShowGraduationImage()
    {
        // 더 이상 시간/스폰 진행 안 되게 정리
        isTimerRunning = false;

        // 플레이어 입력/공격 끄기(선택)
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj)
        {
            var pc = playerObj.GetComponent<PlayerController>();
            if (pc) pc.enabled = false;
            var w = playerObj.GetComponent<Weapon>();
            if (w) w.enabled = false;
        }

        if (graduationImage) graduationImage.SetActive(true);

        if (graduationCanvasGroup)
        {
            float t = 0f, dur = 1.0f;
            graduationCanvasGroup.alpha = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime; 
                graduationCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / dur);
                yield return null;
            }
            graduationCanvasGroup.alpha = 1f;
        }

        
        yield return new WaitForSecondsRealtime(2.0f);
        Time.timeScale = 0f;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 게임 시작 시 연출과 함께 시작
        StartCoroutine(StartGameTransition());
    }

    // 게임 처음 켤 때 문 열리는 연출
    IEnumerator StartGameTransition()
    {
        // 1. 문을 닫은 상태로 시작 (화면 가림)
        leftDoor.anchoredPosition = new Vector2(0, 0);
        rightDoor.anchoredPosition = new Vector2(0, 0);

        yield return new WaitForSeconds(1.0f); // 1초 대기

        // 2. 문 열기 (게임 화면 등장)
        yield return StartCoroutine(AnimateDoors(false));

        // 3. 5층 스테이지 시작
        StartStage(5);
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (currentTimer > 0)
            {
                // [수정] 4층(군대)만의 특별한 시간 흐름 (국방부 시계)
                float timeMultiplier = 1.0f; // 기본은 1배속

                if (currentFloor == 4)
                {
                    timeMultiplier = 0.5f; // 4층은 시간이 0.5배속으로 흐름 (2배 느리게 감)
                }

                // deltaTime에 배율을 곱해서 시간을 깎음
                currentTimer -= Time.deltaTime * timeMultiplier;

                UpdateTimerUI(currentTimer);

                // [룰] 60초 남았을 때 보스 등장
                if (currentTimer <= 60.0f && !isBossSpawned)
                {
                    SpawnBossByFloor(currentFloor);
                    isBossSpawned = true;

                    if (currentFloor == 4) Debug.Log("행보관(보스) 등장! 전역은 멀었다...");
                }
            }
            else
            {
                // 시간 종료 -> 다음 층으로 이동
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
        currentTimer = 120.0f; // 2분 (1분 잡몹 + 1분 보스)
        isTimerRunning = true;

        // UI 갱신
        if (floorText != null) floorText.text = floor + "F";
        Debug.Log(floor + "층 시작! (테마: " + GetFloorTheme(floor) + ")");

        // --- 층별 잡몹 스폰 로직 ---
        if (floor == 5) // 1학년 (자바)
        {
            SpawnEnemy(weakEnemyPrefab, 80);
            SpawnItem(energyDrinkPrefab, 1);
        }
        else if (floor == 4) // 군대 (전역증)
        {
            SpawnEnemy(weakEnemyPrefab, 40); 
            SpawnEnemy(normalEnemyPrefab, 40);
            SpawnItem(energyDrinkPrefab, 1);
            
        }
        else if (floor == 3) // 2학년 (알고리즘)
        {
            SpawnEnemy(normalEnemyPrefab, 40);
            SpawnEnemy(rangedEnemyPrefab, 10);
            SpawnItem(energyDrinkPrefab, 1);
            SpawnItem(cheatSheetPrefab, 1);
        }
        else if (floor == 2) // 3학년 (운영체제)
        {
            SpawnEnemy(normalEnemyPrefab, 40);
            SpawnEnemy(rangedEnemyPrefab, 20);
            SpawnItem(brokenKeyboardPrefab, 1);
        }
        else if (floor == 1) // 4학년 (졸업작품)
        {
            SpawnEnemy(weakEnemyPrefab, 20);
            SpawnEnemy(normalEnemyPrefab, 20);
            SpawnEnemy(rangedEnemyPrefab, 20);

            SpawnItem(energyDrinkPrefab, 3);
            SpawnItem(cheatSheetPrefab, 2);
            SpawnItem(brokenKeyboardPrefab, 2);
        }
    }

    // 층별 보스 소환
    void SpawnBossByFloor(int floor)
    {
        Debug.Log("⚠️ 보스 출현! : " + GetFloorTheme(floor));
        GameObject bossToSpawn = null;

        switch (floor)
        {
            case 5: bossToSpawn = bossJava; break;
            case 4: bossToSpawn = bossArmy; break;
            case 3: bossToSpawn = bossAlgo; break;
            case 2: bossToSpawn = bossOS; break;
            case 1: bossToSpawn = bossGrad; break;
        }

        if (bossToSpawn != null)
        {
            SpawnEnemy(bossToSpawn, 1);
        }

        // 보스 나올 때 잡몹 증원
        SpawnEnemy(weakEnemyPrefab, 5);
    }

    // --- 스테이지 이동 연출 (문 닫힘 -> 이동 -> 문 열림) ---
    IEnumerator NextStageProcess()
    {
        Debug.Log("시험 종료! 문이 닫힙니다.");

        // 1. 문 닫기 (화면 가림)
        yield return StartCoroutine(AnimateDoors(true));

        // 2. 화면 가려진 동안 청소 & 준비
        ClearAllObjects();
        yield return new WaitForSeconds(1.0f); // 잠시 대기 

        int nextFloor = currentFloor - 1;

        if (nextFloor >= 1)
        {
            // 3. 다음 층 데이터 로드
            StartStage(nextFloor);

            // 플레이어 위치 중앙으로 초기화 (선택사항)
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) player.transform.position = Vector3.zero;

            // 4. 문 열기 (짠! 하고 새 스테이지 등장)
            yield return StartCoroutine(AnimateDoors(false));
        }
        else
        {
            Debug.Log("졸업 축하합니다!");
            // 엔딩 처리 (여기선 일단 정지)
            Time.timeScale = 0f;
        }
    }

    // 문 열고 닫는 애니메이션 (수정된 버전)
    IEnumerator AnimateDoors(bool isClosing)
    {
        float timer = 0f;
        float duration = 1.0f; // 1초 동안 이동

        // 왼쪽 문: 닫히면 X=0, 열리면 X=-너비 (왼쪽으로 숨음)
        // (Left Door의 Pivot이 X=0일 경우를 가정. 만약 Pivot이 X=1이면 반대로 계산해야 함)
        // 아까 추천드린 설정(Left Pivot X=0, Right Pivot X=1) 기준입니다.
        float leftTargetX = isClosing ? 0f : -leftDoor.rect.width;

        // 오른쪽 문: 닫히면 X=0, 열리면 X=너비 (오른쪽으로 숨음)
        float rightTargetX = isClosing ? 0f : rightDoor.rect.width;

        // 현재 위치 기억
        Vector2 leftStart = leftDoor.anchoredPosition;
        Vector2 rightStart = rightDoor.anchoredPosition;

        Vector2 leftEnd = new Vector2(leftTargetX, leftStart.y);
        Vector2 rightEnd = new Vector2(rightTargetX, rightStart.y);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // 부드럽게 이동
            leftDoor.anchoredPosition = Vector2.Lerp(leftStart, leftEnd, t);
            rightDoor.anchoredPosition = Vector2.Lerp(rightStart, rightEnd, t);

            yield return null;
        }

        leftDoor.anchoredPosition = leftEnd;
        rightDoor.anchoredPosition = rightEnd;
    }

    // --- 유틸리티 ---
    string GetFloorTheme(int floor)
    {
        switch (floor)
        {
            case 5: return "자바(1학년)";
            case 4: return "전역증(군대)";
            case 3: return "알고리즘(2학년)";
            case 2: return "운영체제(3학년)";
            case 1: return "졸업작품(4학년)";
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
            if (Vector2.Distance(spawnPos, Vector2.zero) > 10.0f) isSafe = true;
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

            // 프로토타입 등 다른 적 스크립트도 있다면 여기서 호출
        }
    }
}