using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; }

    [SerializeField] private GameObject damageTextPrefab;

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

        var go = Instantiate(damageTextPrefab);
        var dt = go.GetComponent<DamageText>();
        if (dt != null)
        {
            dt.Init(damage, isCrit, worldPos);
        }
    }
}
