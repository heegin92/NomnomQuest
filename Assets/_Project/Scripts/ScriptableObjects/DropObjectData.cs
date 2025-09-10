using Ironcow.Synapse.Data;
using System;

[System.Serializable]
public partial class DropObjectData : BaseDataSO
{
    public string target;
    public string displayName;
    public int probability;
    public int minValue;
    public int maxValue;
}