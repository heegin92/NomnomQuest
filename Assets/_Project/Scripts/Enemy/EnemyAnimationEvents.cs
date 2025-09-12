using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void OnAttackHit()
    {
        if (enemy != null)
            enemy.OnAttackHit();
    }
}
