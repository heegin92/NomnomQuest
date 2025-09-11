using Ironcow.Synapse.Data;
using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("�⺻ ����")]
    public string rcode;
    public string displayName;
    [TextArea]
    public string description;

    [Header("�ɷ�ġ")]
    public int maxHp;
    public int def;
    public int atk;
    public float walkSpeed;

    [Header("����")]
    public int exp;
    public int gold;

    [Header("��� ������")]
    public ItemData[] dropItems;

    [Header("���־�")]
    public GameObject prefab;
}