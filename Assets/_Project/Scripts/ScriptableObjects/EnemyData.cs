using Ironcow.Synapse.Data;
using System;
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
    public string DropItems;
    internal GameObject prefab;
    internal string dropItems;
}