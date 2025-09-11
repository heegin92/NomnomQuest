using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        // �θ� ������Ʈ(Player.cs) ã�Ƽ� ĳ��
        player = GetComponentInParent<Player>();
    }

    // Animation Event���� ȣ��Ǵ� �Լ�
    public void OnAttackHit()
    {
        if (player != null)
            player.OnAttackHit();
    }
}
