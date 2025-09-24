using System.Collections.Generic;
using UnityEngine;
#if CINEMACHINE
using Cinemachine;
#endif

public class SceneLoaderStage : MonoBehaviour
{
    [Header("스테이지 씬 설정")]
    [SerializeField] private int stageNum = 1;

    [Tooltip("플레이어 프리팹 (필수)")]
    [SerializeField] private GameObject playerPrefab;

    [Tooltip("스폰 지점 (없으면 Fallback 사용)")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Vector3 playerSpawnPosFallback = Vector3.zero;

    [Tooltip("스테이지 시작 시 재생할 BGM (선택)")]
    [SerializeField] private AudioClip bgSoundClip;

    private Transform mapSpawnPoint;           // 맵 내부 SpawnPoint 참조
    private List<EnemySpawner> stageSpawners;  // 맵 내부 EnemySpawner 리스트

    private void Awake()
    {
        // ✅ 맵 로드
        int stageNum = GameManager.Instance.CurrentStage;
        LoadMapPrefab(stageNum);

        if (playerPrefab == null)
        {
            Debug.LogError("[SceneLoaderStage] Player Prefab이 비어있습니다.");
            return;
        }

        // ✅ 스폰 위치 계산
        Vector3 spawnPos = mapSpawnPoint
            ? mapSpawnPoint.position
            : (playerSpawnPoint ? playerSpawnPoint.position : playerSpawnPosFallback);

        // ✅ Player 생성 or 재사용
        if (GameManager.Instance.Player == null)
        {
            GameObject newPlayerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            Player newPlayer = newPlayerObj.GetComponent<Player>();

            if (newPlayer == null)
            {
                Debug.LogError("[SceneLoaderStage] Player Prefab에 Player 스크립트 없음!");
                return;
            }

            GameManager.Instance.Player = newPlayer;
            GameManager.Instance.CurrentStage = stageNum;
            Debug.Log("[SceneLoaderStage] Player 생성 & GameManager 등록 완료");
        }
        else
        {
            GameManager.Instance.Player.transform.position = spawnPos;
            Debug.Log("[SceneLoaderStage] 기존 Player 재사용");
        }

        // ✅ BGM 재생
        if (bgSoundClip != null && SoundManager.Instance != null)
            SoundManager.Instance.ChangeBackGroundMusic(bgSoundClip);

        // ✅ 카메라 Follow 연결
#if CINEMACHINE
        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null && GameManager.Instance.Player != null)
        {
            vcam.Follow = GameManager.Instance.Player.transform;
            vcam.LookAt = null;
            Debug.Log("[SceneLoaderStage] VCam Follow 연결됨");
        }
        else
        {
            Debug.LogWarning("[SceneLoaderStage] VCam 없음");
        }
#endif

        // ✅ EnemySpawner 시작 (전투 스테이지일 때만)
        if (stageSpawners != null && stageSpawners.Count > 0)
        {
            foreach (var spawner in stageSpawners)
                spawner.BeginSpawn();

            Debug.Log($"[SceneLoaderStage] {stageSpawners.Count}개의 EnemySpawner 시작 완료");
        }
        else
        {
            Debug.Log("[SceneLoaderStage] EnemySpawner 없음 → 마을 모드");
        }
    }

    private void Start()
    {
        Debug.Log("[SceneLoaderStage] Start() 진입 - 데이터 로드 시작");

        if (DataManager.Instance != null && DataManager.Instance.userInfo != null && GameManager.Instance.Player != null)
        {
            var p = GameManager.Instance.Player;
            var info = DataManager.Instance.userInfo;

            p.data.level = info.level;
            p.data.exp = info.exp;
            p.data.expToNextLevel = info.expToNextLevel;
            p.data.attack = info.attack;
            p.data.maxHealth = info.maxHealth;
            p.data.health = Mathf.Clamp(info.health, 0, p.data.maxHealth);
            p.data.gold = info.gold;

            Debug.Log($"[SceneLoaderStage] Player 데이터 로드 완료 → HP {p.data.health}/{p.data.maxHealth}, EXP {p.data.exp}/{p.data.expToNextLevel}, GOLD {p.data.gold}");
        }
        else
        {
            Debug.LogWarning("[SceneLoaderStage] 동기화 조건 불충족 → 스킵됨");
        }
    }

    private void LoadMapPrefab(int stage)
    {
        string path = $"Maps/Stage_{stage:D3}";   // 예: Stage_001
        GameObject mapPrefab = Resources.Load<GameObject>(path);

        if (mapPrefab == null)
        {
            Debug.LogWarning($"[SceneLoaderStage] 맵 {path} 없음 → 기본 Stage_001 로드");
            mapPrefab = Resources.Load<GameObject>("Maps/Stage_001");
        }

        if (mapPrefab != null)
        {
            var map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log($"[SceneLoaderStage] 맵 로드 완료: {mapPrefab.name}");

            // ✅ 맵 안에서 SpawnPoint 찾기
            var spawn = map.transform.Find("SpawnPoint");
            if (spawn != null)
            {
                mapSpawnPoint = spawn;
                Debug.Log("[SceneLoaderStage] 맵 SpawnPoint 연결 완료");
            }

            // ✅ 맵 안에서 EnemySpawner 찾기
            stageSpawners = new List<EnemySpawner>(map.GetComponentsInChildren<EnemySpawner>());
        }
        else
        {
            Debug.LogError($"[SceneLoaderStage] 맵 프리팹을 끝내 못 찾음 (Stage_{stage:D3} & Stage_001 둘 다 없음)");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 p = mapSpawnPoint ? mapSpawnPoint.position :
                   (playerSpawnPoint ? playerSpawnPoint.position : playerSpawnPosFallback);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(p, 0.25f);
        Gizmos.DrawWireCube(p + Vector3.up * 1.0f, new Vector3(0.5f, 2f, 0.5f));
    }
}
