using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        // 부모 오브젝트(Player.cs) 찾아서 캐싱
        player = GetComponentInParent<Player>();
    }

    // Animation Event에서 호출되는 함수
    public void OnAttackHit()
    {
        if (player != null)
            player.OnAttackHit();
    }
}
