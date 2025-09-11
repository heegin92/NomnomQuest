using Ironcow.Synapse.Data;
using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("기본 정보")]
    public string rcode;
    public string displayName;
    [TextArea]
    public string description;

    [Header("능력치")]
    public int maxHp;
    public int def;
    public int atk;
    public float walkSpeed;

    [Header("보상")]
    public int exp;
    public int gold;

    [Header("드랍 아이템")]
    public ItemData[] dropItems;

    [Header("비주얼")]
    public GameObject prefab;
}