using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Cinemachine을 쓰면 자동 연동됩니다. (없어도 컴파일 OK)
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
    [Tooltip("플레이어 스폰 위치(없으면 (0,0,0) 사용)")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Vector3 playerSpawnPosFallback = Vector3.zero;

    [Header("카메라 옵션")]
    [Tooltip("시작 시 플레이어를 카메라 중앙으로 스냅")]
    [SerializeField] private bool snapCameraToPlayerOnStart = true;

    [Tooltip("플레이어가 이미 존재하면 재사용(위치만 이동). false면 항상 새로 생성")]
    [SerializeField] private bool reuseExistingPlayerIfAny = true;

    private void Awake()
    {
        // === Player 준비 ===
        if (playerPrefab == null)
        {
            Debug.LogError("[SceneLoaderStage] Player Prefab이 비어있습니다.");
            return;
        }

        Vector3 spawnPos = playerSpawnPoint ? playerSpawnPoint.position : playerSpawnPosFallback;

        Player player = null;

        if (reuseExistingPlayerIfAny && GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            // 기존 플레이어 재사용: 위치만 이동/초기화
            player = GameManager.Instance.Player;
            var pTr = player.transform;
            pTr.position = spawnPos;
            pTr.rotation = Quaternion.identity;
            ResetPlayerStateIfNeeded(player);
        }
        else
        {
            // 새로 생성
            var go = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            player = go.GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("[SceneLoaderStage] Player 컴포넌트를 찾을 수 없습니다. 프리팹에 Player가 붙어있는지 확인하세요.");
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Player = player;
            }
        }

        // === BGM 변경 ===
        if (bgSoundClip != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.ChangeBackGroundMusic(bgSoundClip);
        }

        // === 카메라 세팅(시네머신 우선) ===
        if (snapCameraToPlayerOnStart && player != null)
        {
#if CINEMACHINE
    var vcam = FindObjectOfType<CinemachineVirtualCamera>();
    if (!vcam) return;

    // 즉시 스냅: 이전 상태 무효화
    vcam.PreviousStateIsValid = false;

    // Do Nothing 에서 카메라 회전 고정
    vcam.transform.rotation = Quaternion.identity;

    // Follow 잠시 해제 → 위치 강제 세팅 → 다시 Follow
    var followBak = vcam.Follow;
    vcam.Follow = null;
    var camZ = vcam.transform.position.z;
    vcam.transform.position = new Vector3(player.position.x, player.position.y, camZ);
    vcam.Follow = player;
#endif
            {
                // 시네머신이 없으면 메인 카메라를 즉시 스냅
                var cam = Camera.main;
                if (cam != null)
                {
                    var camPos = cam.transform.position;
                    cam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, camPos.z);
                }
            }
        }
    }

    private void Start()
    {

        // === HUD에 스테이지 번호 표시 ===
        if (GameManager.Instance != null && GameManager.Instance.Player != null && GameManager.Instance.Player.HUD != null)
        {
            GameManager.Instance.Player.HUD.SetStageText(stageNum);
        }
    }

    /// <summary>
    /// 필요 시 플레이어 상태 초기화(체력/스태미나/애니/입력락 등)
    /// 프로젝트 규칙에 맞게 내부 내용 수정해서 쓰세요.
    /// </summary>
    private void ResetPlayerStateIfNeeded(Player player)
    {
        // 예시) player.ResetForStage(); 처럼 프로젝트 메서드가 있으면 호출
        // 없으면 아래처럼 안전한 기본 초기화만:
        var rb = player.GetComponent<Rigidbody>();
        if (rb) rb.velocity = Vector3.zero;

        var rb2d = player.GetComponent<Rigidbody2D>();
        if (rb2d) rb2d.velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        // 스폰 지점 가시화
        Vector3 p = playerSpawnPoint ? playerSpawnPoint.position : playerSpawnPosFallback;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(p, 0.25f);
        Gizmos.DrawWireCube(p + Vector3.up * 1.0f, new Vector3(0.5f, 2f, 0.5f));
    }
}
