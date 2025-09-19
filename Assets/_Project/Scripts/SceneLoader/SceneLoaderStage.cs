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

    private void Awake()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[SceneLoaderStage] Player Prefab이 비어있습니다.");
            return;
        }

        // ✅ 스폰 위치 계산
        Vector3 spawnPos = playerSpawnPoint ? playerSpawnPoint.position : playerSpawnPosFallback;

        // ✅ Player 생성 & GameManager에 등록
        var go = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        if (go == null)
        {
            Debug.LogError("[SceneLoaderStage] Instantiate 실패");
            return;
        }
        var player = go.GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError("[SceneLoaderStage] Player Prefab에 Player 스크립트 없음!");
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Player = player;
            GameManager.Instance.CurrentStage = stageNum;
            Debug.Log("[SceneLoaderStage] Player 생성 & GameManager 등록 완료");
        }
        else
        {
            Debug.LogError("[SceneLoaderStage] GameManager.Instance == null");
        }

        // ✅ BGM 재생
        if (bgSoundClip != null && SoundManager.Instance != null)
            SoundManager.Instance.ChangeBackGroundMusic(bgSoundClip);

        // ✅ 카메라 Follow 연결
#if CINEMACHINE
        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.Follow = player.transform;
            vcam.LookAt = null;
            Debug.Log("[SceneLoaderStage] VCam Follow 연결됨");
        }
        else
        {
            Debug.LogWarning("[SceneLoaderStage] VCam 없음");
        }
#endif

        // ✅ EnemySpawner에게 스폰 시작 신호 보내기
        var spawners = FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in spawners)
        {
            spawner.BeginSpawn();
        }
        Debug.Log("[SceneLoaderStage] EnemySpawner 시작 신호 전송 완료");
    }


    private void Start()
    {
        Debug.Log("[SceneLoaderStage] Start() 진입 - 데이터 로드 시작");

        Debug.Log($"DataManager.Instance = {(DataManager.Instance != null ? "O" : "X")}");
        Debug.Log($"DataManager.userInfo = {(DataManager.Instance != null && DataManager.Instance.userInfo != null ? "O" : "X")}");
        Debug.Log($"GameManager.Instance = {(GameManager.Instance != null ? "O" : "X")}");
        Debug.Log($"GameManager.Player = {(GameManager.Instance != null && GameManager.Instance.Player != null ? "O" : "X")}");

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


    private void OnDrawGizmosSelected()
    {
        Vector3 p = playerSpawnPoint ? playerSpawnPoint.position : playerSpawnPosFallback;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(p, 0.25f);
        Gizmos.DrawWireCube(p + Vector3.up * 1.0f, new Vector3(0.5f, 2f, 0.5f));
    }
}
