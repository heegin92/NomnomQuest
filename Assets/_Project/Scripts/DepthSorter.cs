using UnityEngine;
using UnityEngine.Rendering; // SortingGroup

[DisallowMultipleComponent]
public class DepthSorter : MonoBehaviour
{
    private SpriteRenderer sr;
    private SortingGroup sg;

    [Tooltip("���е� ��� (���� Ŭ���� �����ϰ� ���ĵ�)")]
    [SerializeField] private int precision = 100;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sg = GetComponent<SortingGroup>();
    }

    private void LateUpdate()
    {
        int order = -(int)(transform.position.y * precision);

        if (sr != null)
        {
            sr.sortingOrder = order;
        }
        else if (sg != null)
        {
            sg.sortingOrder = order;
        }
    }
}
