using System.Collections;
using UnityEngine;
#if CINEMACHINE
using Cinemachine;
#endif

public class SceneLoaderStage : MonoBehaviour
{
    [Header("�������� �� ����")]
    [SerializeField] private int stageNum = 1;

    [Tooltip("�÷��̾� ������ (�ʼ�)")]
    [SerializeField] private GameObject playerPrefab;

    [Tooltip("�������� ���� �� ����� BGM (����)")]
    [SerializeField] private AudioClip bgSoundClip;

    [Header("���� ����")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Vector3 playerSpawnPosFallback = Vector3.zero;

    [Header("ī�޶� �ɼ�")]
    [SerializeField] private bool snapCameraToPlayerOnStart = true;
    [SerializeField] private bool reuseExistingPlayerIfAny = true;

    [Tooltip("ī�޶� ������ (isometric ���� ����)")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 10, -10);

    [Tooltip("ī�޶� ���� �ε巯��")]
    [Range(0, 10f)][SerializeField] private float cameraDamping = 2f;

    private void Awake()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[SceneLoaderStage] Player Prefab�� ����ֽ��ϴ�.");
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

        // === ī�޶� Follow �ڵ� ���� ===
#if CINEMACHINE
    var vcam = FindObjectOfType<CinemachineVirtualCamera>();
    if (vcam != null && player != null)
    {
        Debug.Log("[SceneLoaderStage] VCam Follow �����");
        vcam.Follow = player.transform;
        vcam.LookAt = null;
    }
    else
    {
        Debug.LogWarning("[SceneLoaderStage] VCam ���� �Ǵ� Player ����");
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
