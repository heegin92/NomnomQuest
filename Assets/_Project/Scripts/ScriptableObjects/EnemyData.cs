using System;
using Ironcow.Synapse.Data;
using UnityEngine;

[System.Serializable]
public partial class EnemyData : BaseDataSO
{
    public string displayName;
    public string description;
    public int hp;
    public int def;
    public int atk;
    public int walkSpeed;
    public int exp;
    public int gold;
    public int detectRange;
    public int atkRange;
    public int attackCooldown;

    [Header("Prefab")]
    public GameObject prefab;   // ✅ 스포너에서 쓸 프리팹 연결
    public string DropItems;
}