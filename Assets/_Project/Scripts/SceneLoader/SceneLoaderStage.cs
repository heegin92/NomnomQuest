using System.Collections;
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

    [Tooltip("스테이지 시작 시 재생할 BGM (선택)")]
    [SerializeField] private AudioClip bgSoundClip;

    [Header("스폰 정보")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Vector3 playerSpawnPosFallback = Vector3.zero;

    [Header("카메라 옵션")]
    [SerializeField] private bool snapCameraToPlayerOnStart = true;
    [SerializeField] private bool reuseExistingPlayerIfAny = true;

    [Tooltip("카메라 오프셋 (isometric 느낌 조정)")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 10, -10);

    [Tooltip("카메라 추적 부드러움")]
    [Range(0, 10f)][SerializeField] private float cameraDamping = 2f;

    private void Awake()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[SceneLoaderStage] Player Prefab이 비어있습니다.");
            return;
        }

        Vector3 spawnPos = playerSpawnPoint ? playerSpawnPoint.position : playerSpawnPosFallback;
        Player player = null;

        if (reuseExistingPlayerIfAny && GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            player = GameManager.Instance.Player;
            var pTr = player.transform;
            pTr.position = spawnPos;
            pTr.rotation = Quaternion.identity;
            ResetPlayerStateIfNeeded(player);
        }
        else
        {
            var go = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            player = go.GetComponent<Player>();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Player = player;
            }
        }

        // === BGM ===
        if (bgSoundClip != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.ChangeBackGroundMusic(bgSoundClip);
        }

        // === 카메라 Follow 자동 연결 ===
#if CINEMACHINE
    var vcam = FindObjectOfType<CinemachineVirtualCamera>();
    if (vcam != null && player != null)
    {
        Debug.Log("[SceneLoaderStage] VCam Follow 연결됨");
        vcam.Follow = player.transform;
        vcam.LookAt = null;
    }
    else
    {
        Debug.LogWarning("[SceneLoaderStage] VCam 없음 또는 Player 없음");
    }
#endif
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null && GameManager.Instance.Player.HUD != null)
        {
            GameManager.Instance.Player.HUD.SetStageText(stageNum);
        }
    }

    private void ResetPlayerStateIfNeeded(Player player)
    {
        var rb = player.GetComponent<Rigidbody>();
        if (rb) rb.velocity = Vector3.zero;

        var rb2d = player.GetComponent<Rigidbody2D>();
        if (rb2d) rb2d.velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 p = playerSpawnPoint ? playerSpawnPoint.position : playerSpawnPosFallback;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(p, 0.25f);
        Gizmos.DrawWireCube(p + Vector3.up * 1.0f, new Vector3(0.5f, 2f, 0.5f));
    }
}
