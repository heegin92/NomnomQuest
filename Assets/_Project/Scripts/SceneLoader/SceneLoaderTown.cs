using UnityEngine;
#if CINEMACHINE
using Cinemachine;
#endif

public class SceneLoaderTown : MonoBehaviour
{
    [Header("마을 설정")]
    [SerializeField] private GameObject townPrefab;   // ✅ 마을 환경 프리팹
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;    // ✅ 비어있으면 (0,0,0)으로 스폰
    [SerializeField] private AudioClip townBGM;

    private void Awake()
    {
        // ✅ 마을 환경 배치
        if (townPrefab != null)
            Instantiate(townPrefab, Vector3.zero, Quaternion.identity);

        if (playerPrefab == null)
        {
            Debug.LogError("[SceneLoaderTown] Player Prefab이 비어있습니다.");
            return;
        }

        // ✅ 스폰 위치 (Fallback 지원)
        Vector3 spawnPos = spawnPoint ? spawnPoint.position : Vector3.zero;

        // ✅ 플레이어 생성
        var player = Instantiate(playerPrefab, spawnPos, Quaternion.identity).GetComponent<Player>();

        // ✅ GameManager 등록
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Player = player;
            GameManager.Instance.CurrentStage = 0;   // 0 = 마을
            GameManager.Instance.IsTown = true;

            // ✅ DataManager에서 데이터 불러오기
            if (DataManager.Instance != null && DataManager.Instance.userInfo != null)
            {
                var info = DataManager.Instance.userInfo;
                player.data.level = info.level;
                player.data.exp = info.exp;
                player.data.expToNextLevel = info.expToNextLevel;
                player.data.attack = info.attack;
                player.data.maxHealth = info.maxHealth;
                player.data.health = Mathf.Clamp(info.health, 0, player.data.maxHealth);
                player.data.gold = info.gold;

                Debug.Log($"[SceneLoaderTown] Player 데이터 로드 완료 → HP {player.data.health}/{player.data.maxHealth}, EXP {player.data.exp}/{player.data.expToNextLevel}, GOLD {player.data.gold}");
            }
        }

        // ✅ HUD 연결
        var hud = FindObjectOfType<PlayerHUD>();
        if (hud != null)
        {
            hud.SetPlayer(player);
            Debug.Log("[SceneLoaderTown] PlayerHUD 연결 완료");
        }
        else
        {
            Debug.LogWarning("[SceneLoaderTown] PlayerHUD를 씬에서 찾지 못함");
        }

        // ✅ BGM 실행
        if (townBGM != null && SoundManager.Instance != null)
            SoundManager.Instance.ChangeBackGroundMusic(townBGM);

#if CINEMACHINE
        // ✅ 카메라 연결
        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.Follow = player.transform;
            vcam.LookAt = null;
            Debug.Log("[SceneLoaderTown] VCam Follow 연결됨");
        }
#endif
    }
}
