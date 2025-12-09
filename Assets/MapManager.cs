using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("층별 맵 프리팹 (5층~1층)")]
    public GameObject[] floorMaps; // 0=5층, 1=4층 ...

    [Header("플레이어 오브젝트")]
    public GameObject player;

    private GameObject currentMap;
    private int currentFloor = 5;
    private int currentRound = 1;

    private float timer = 0f;

    void Start()
    {
        LoadCurrentFloorMap();
    }

    void Update()
    {
       
        timer += Time.unscaledDeltaTime;

        float duration = GetCurrentStageDuration();

        if (timer >= duration)
        {
            timer = 0;
            NextRound();
        }
    }

  
    float GetCurrentStageDuration()
    {
        switch (currentFloor)
        {
            case 4:
                return 120f; 
            default:
                return 64f; 
        }
    }

    void LoadCurrentFloorMap()
    {
        if (currentMap != null)
            Destroy(currentMap);

        int index = 5 - currentFloor;
        if (index < 0 || index >= floorMaps.Length)
        {
            Debug.LogError("잘못된 층 번호!");
            return;
        }

        // 맵 생성
        currentMap = Instantiate(floorMaps[index]);

        // 플레이어 스폰
        Transform spawn = currentMap.transform.Find("PlayerSpawnPoint");
        if (spawn != null && player != null)
            player.transform.position = spawn.position;

        Debug.Log($"[{currentFloor}층] 맵 로드 완료");
    }

    void NextRound()
    {
        if (currentRound == 1)
        {
            currentRound = 2;
            Debug.Log($"{currentFloor}층 2라운드 시작!");
            // 보스 등장 / 음악 변경 등 넣을 수 있음
        }
        else
        {
            currentRound = 1;
            currentFloor--;

            if (currentFloor < 1)
            {
                Debug.Log("모든 층 클리어!");
                return;
            }

            LoadCurrentFloorMap();
            Debug.Log($"{currentFloor}층 1라운드 시작!");
        }
    }
}
