using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Cinemachine�� ���� �ڵ� �����˴ϴ�. (��� ������ OK)
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
    [Tooltip("�÷��̾� ���� ��ġ(������ (0,0,0) ���)")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Vector3 playerSpawnPosFallback = Vector3.zero;

    [Header("ī�޶� �ɼ�")]
    [Tooltip("���� �� �÷��̾ ī�޶� �߾����� ����")]
    [SerializeField] private bool snapCameraToPlayerOnStart = true;

    [Tooltip("�÷��̾ �̹� �����ϸ� ����(��ġ�� �̵�). false�� �׻� ���� ����")]
    [SerializeField] private bool reuseExistingPlayerIfAny = true;

    private void Awake()
    {
        // === Player �غ� ===
        if (playerPrefab == null)
        {
            Debug.LogError("[SceneLoaderStage] Player Prefab�� ����ֽ��ϴ�.");
            return;
        }

        Vector3 spawnPos = playerSpawnPoint ? playerSpawnPoint.position : playerSpawnPosFallback;

        Player player = null;

        if (reuseExistingPlayerIfAny && GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            // ���� �÷��̾� ����: ��ġ�� �̵�/�ʱ�ȭ
            player = GameManager.Instance.Player;
            var pTr = player.transform;
            pTr.position = spawnPos;
            pTr.rotation = Quaternion.identity;
            ResetPlayerStateIfNeeded(player);
        }
        else
        {
            // ���� ����
            var go = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            player = go.GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("[SceneLoaderStage] Player ������Ʈ�� ã�� �� �����ϴ�. �����տ� Player�� �پ��ִ��� Ȯ���ϼ���.");
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Player = player;
            }
        }

        // === BGM ���� ===
        if (bgSoundClip != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.ChangeBackGroundMusic(bgSoundClip);
        }

        // === ī�޶� ����(�ó׸ӽ� �켱) ===
        if (snapCameraToPlayerOnStart && player != null)
        {
#if CINEMACHINE
    var vcam = FindObjectOfType<CinemachineVirtualCamera>();
    if (!vcam) return;

    // ��� ����: ���� ���� ��ȿȭ
    vcam.PreviousStateIsValid = false;

    // Do Nothing ���� ī�޶� ȸ�� ����
    vcam.transform.rotation = Quaternion.identity;

    // Follow ��� ���� �� ��ġ ���� ���� �� �ٽ� Follow
    var followBak = vcam.Follow;
    vcam.Follow = null;
    var camZ = vcam.transform.position.z;
    vcam.transform.position = new Vector3(player.position.x, player.position.y, camZ);
    vcam.Follow = player;
#endif
            {
                // �ó׸ӽ��� ������ ���� ī�޶� ��� ����
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

        // === HUD�� �������� ��ȣ ǥ�� ===
        if (GameManager.Instance != null && GameManager.Instance.Player != null && GameManager.Instance.Player.HUD != null)
        {
            GameManager.Instance.Player.HUD.SetStageText(stageNum);
        }
    }

    /// <summary>
    /// �ʿ� �� �÷��̾� ���� �ʱ�ȭ(ü��/���¹̳�/�ִ�/�Է¶� ��)
    /// ������Ʈ ��Ģ�� �°� ���� ���� �����ؼ� ������.
    /// </summary>
    private void ResetPlayerStateIfNeeded(Player player)
    {
        // ����) player.ResetForStage(); ó�� ������Ʈ �޼��尡 ������ ȣ��
        // ������ �Ʒ�ó�� ������ �⺻ �ʱ�ȭ��:
        var rb = player.GetComponent<Rigidbody>();
        if (rb) rb.velocity = Vector3.zero;

        var rb2d = player.GetComponent<Rigidbody2D>();
        if (rb2d) rb2d.velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        // ���� ���� ����ȭ
        Vector3 p = playerSpawnPoint ? playerSpawnPoint.position : playerSpawnPosFallback;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(p, 0.25f);
        Gizmos.DrawWireCube(p + Vector3.up * 1.0f, new Vector3(0.5f, 2f, 0.5f));
    }
}
