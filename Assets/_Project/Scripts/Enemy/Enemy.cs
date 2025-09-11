using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("데이터 참조")]
    public EnemyData data;   // SO 참조

    [Header("런타임 상태")]
    private int currentHp;

    private Renderer rend;           // 적의 Renderer
    private Color originalColor;     // 원래 색상 저장


    private void Awake()
    {
        if (data != null)
            currentHp = data.maxHp;  // SO에서 초기화
    }

    public void TakeDamage(int dmg)
    {
        int finalDamage = Mathf.Max(0, dmg - data.def);
        currentHp -= finalDamage;

        Debug.Log($"{data.displayName} 피격! HP: {currentHp}/{data.maxHp}");

        // 깜빡이 효과 실행
        if (rend != null)
            StartCoroutine(HitFlash());

        if (IsDead())
            Die();
    }

    public bool IsDead() => currentHp <= 0;

    public void Die()
    {
        Debug.Log($"{data.displayName} 사망! EXP {data.exp}, Gold {data.gold}");

        DropLoot();
        Destroy(gameObject);
    }
    private IEnumerator HitFlash()
    {
        // 빨간색으로 변경
        rend.material.color = Color.red;

        // 0.1초 정도 유지
        yield return new WaitForSeconds(0.1f);

        // 원래 색상 복원
        rend.material.color = originalColor;
    }
    private void DropLoot()
    {
        if (data.dropItems == null || data.dropItems.Length == 0) return;

        foreach (var item in data.dropItems)
        {
            if (item == null) continue;
            Debug.Log($"드랍: {item.displayName} (코드: {item.rcode})");
            // TODO: 인벤토리나 필드 드랍 연동
        }
    }
}
