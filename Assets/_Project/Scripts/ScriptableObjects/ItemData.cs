using Ironcow.Synapse.Data;
using System;

[System.Serializable]
public partial class ItemData : BaseDataSO
{
    public string displayName;
    public string description;
    public int type;
    public bool canStack;
    public int maxStackAmount;
    public int hp;
    public int mp;
    public int hungry;
    public int effect;
    public string equipPrefab;
}