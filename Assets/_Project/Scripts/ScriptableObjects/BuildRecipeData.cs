using Ironcow.Synapse.Data;
using System;

[System.Serializable]
public partial class BuildRecipeData : BaseDataSO
{
    public string displayName;
    public string materials;
    public int counts;
    public string createItem;
}