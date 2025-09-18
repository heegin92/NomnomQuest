using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; }

    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private GameObject critEffectPrefab; // ⭐ 치명타 파티클 프리팹

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowDamage(int damage, Vector3 worldPos, bool isCrit = false)
    {
        if (damageTextPrefab == null) return;

        GameObject go = Instantiate(damageTextPrefab, worldPos, Quaternion.identity);
        var dmg = go.GetComponent<DamageText>();
        if (dmg != null)
        {
            dmg.Init(damage, isCrit, worldPos);
        }

        // 치명타일 경우 파티클도 추가
        if (isCrit && critEffectPrefab != null)
        {
            GameObject fx = Instantiate(critEffectPrefab, worldPos + Vector3.up * 1.2f, Quaternion.identity);
            Destroy(fx, 1.5f); // 1.5초 후 자동 제거
        }
    }
}
