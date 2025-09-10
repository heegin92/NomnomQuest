using Ironcow.Synapse.Data;
using System;

[System.Serializable]
public partial class WorldObjectData : BaseDataSO
{
    public string displayName;
    public string description;
    public string hp;
    public string dropItems;
    public string possibleEquip;
}