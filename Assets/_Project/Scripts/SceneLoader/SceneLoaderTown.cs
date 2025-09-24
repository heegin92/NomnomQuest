using System.Collections;
using UnityEngine;
#if CINEMACHINE
using Cinemachine;
#endif

public class SceneLoaderTown : MonoBehaviour
{
    [Header("마을 설정")]
    [SerializeField] private GameObject townPrefab;   // 마을 환경 프리팹(선택)
    [SerializeField] private GameObject playerPrefab; // Player 프리팹(필수)
    [SerializeField] private Transform spawnPoint;    // 스폰 위치
    [SerializeField] private Vector3 spawnFallback = Vector3.zero;
    [SerializeField] private AudioClip townBGM;

    private void Awake()
    {
        // ✅ 마을 프리팹 로드
        if (townPrefab != null)
            Instantiate(townPrefab, Vector3.zero, Quaternion.identity);

        if (playerPrefab == null)
        {
            Debug.LogError("[SceneLoaderTown] Player Prefab이 비어있습니다.");
            return;
        }

        // ✅ 스폰 위치 계산
        Vector3 spawnPos = spawnPoint ? spawnPoint.position : spawnFallback;

        // ✅ Player 생성 or 재사용
        Player player = null;
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.Player == null)
            {
                Debug.Log("[SceneLoaderTown] Player 없음 → 새로 생성");
                var newPlayerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                player = newPlayerObj.GetComponent<Player>();

                if (player == null)
                {
                    Debug.LogError("[SceneLoaderTown] Player Prefab에 Player 스크립트 없음!");
                    return;
                }

                GameManager.Instance.Player = player;
            }
            else
            {
                Debug.Log("[SceneLoaderTown] 기존 Player 재사용");
                player = GameManager.Instance.Player;
                player.transform.position = spawnPos;
            }

            GameManager.Instance.CurrentStage = 0; // 0 = 마을
            GameManager.Instance.IsTown = true;
        }
        else
        {
            Debug.LogError("[SceneLoaderTown] GameManager.Instance == null");
            return;
        }

        // ✅ 데이터 즉시 동기화
        SyncPlayerData(GameManager.Instance.Player);

        // ✅ HUD 연결
        var hud = FindObjectOfType<PlayerHUD>();
        if (hud != null)
        {
            hud.SetPlayer(player);
            if (hud.goldText != null)
                hud.goldText.text = $"{player.data.gold} G";
        }

        // ✅ BGM
        if (townBGM != null && SoundManager.Instance != null)
            SoundManager.Instance.ChangeBackGroundMusic(townBGM);

        // ✅ 카메라 Follow
#if CINEMACHINE
        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null && player != null)
        {
            vcam.Follow = player.transform;
            vcam.LookAt = null;
        }
#endif
    }

    private void Start()
    {
        // Player.Start()에서 초기화가 덮어쓰는 걸 방지하기 위해
        StartCoroutine(ResyncAfterOneFrame());
    }

    private IEnumerator ResyncAfterOneFrame()
    {
        yield return null; // 한 프레임 대기
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            SyncPlayerData(GameManager.Instance.Player);

            var hud = FindObjectOfType<PlayerHUD>();
            if (hud != null)
            {
                hud.SetPlayer(GameManager.Instance.Player);
                if (hud.goldText != null)
                    hud.goldText.text = $"{GameManager.Instance.Player.data.gold} G";
            }
        }
    }

    private void SyncPlayerData(Player p)
    {
        if (p == null) return;

        if (DataManager.Instance != null && DataManager.Instance.userInfo != null)
        {
            var info = DataManager.Instance.userInfo;

            p.data.level = info.level;
            p.data.exp = info.exp;
            p.data.expToNextLevel = info.expToNextLevel;
            p.data.attack = info.attack;
            p.data.maxHealth = info.maxHealth;
            p.data.health = Mathf.Clamp(info.health, 0, p.data.maxHealth);
            p.data.gold = info.gold;

            Debug.Log($"[SceneLoaderTown] 동기화 완료 → LV {p.data.level}, HP {p.data.health}/{p.data.maxHealth}, EXP {p.data.exp}/{p.data.expToNextLevel}, GOLD {p.data.gold}");
        }
        else
        {
            Debug.LogWarning("[SceneLoaderTown] DataManager.userInfo 없음 → 동기화 스킵");
        }
    }
}
