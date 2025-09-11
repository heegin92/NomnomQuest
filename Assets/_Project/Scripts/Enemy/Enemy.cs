using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("������ ����")]
    public EnemyData data;   // SO ����

    [Header("��Ÿ�� ����")]
    private int currentHp;

    private Renderer rend;           // ���� Renderer
    private Color originalColor;     // ���� ���� ����


    private void Awake()
    {
        if (data != null)
            currentHp = data.maxHp;  // SO���� �ʱ�ȭ
    }

    public void TakeDamage(int dmg)
    {
        int finalDamage = Mathf.Max(0, dmg - data.def);
        currentHp -= finalDamage;

        Debug.Log($"{data.displayName} �ǰ�! HP: {currentHp}/{data.maxHp}");

        // ������ ȿ�� ����
        if (rend != null)
            StartCoroutine(HitFlash());

        if (IsDead())
            Die();
    }

    public bool IsDead() => currentHp <= 0;

    public void Die()
    {
        Debug.Log($"{data.displayName} ���! EXP {data.exp}, Gold {data.gold}");

        DropLoot();
        Destroy(gameObject);
    }
    private IEnumerator HitFlash()
    {
        // ���������� ����
        rend.material.color = Color.red;

        // 0.1�� ���� ����
        yield return new WaitForSeconds(0.1f);

        // ���� ���� ����
        rend.material.color = originalColor;
    }
    private void DropLoot()
    {
        if (data.dropItems == null || data.dropItems.Length == 0) return;

        foreach (var item in data.dropItems)
        {
            if (item == null) continue;
            Debug.Log($"���: {item.displayName} (�ڵ�: {item.rcode})");
            // TODO: �κ��丮�� �ʵ� ��� ����
        }
    }
}
