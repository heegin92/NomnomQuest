using Ironcow.Synapse.Data;
using System;

[System.Serializable]
public partial class EnemyData : BaseDataSO
{
    public string displayName;
    public string description;
    public int hp;
    public int def;
    public int atk;
    public int walkSpeed;
    public int runSpeed;
    public string DropItems;
}