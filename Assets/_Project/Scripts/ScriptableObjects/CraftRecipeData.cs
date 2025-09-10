using Ironcow.Synapse.Data;
using System;

[System.Serializable]
public partial class CraftRecipeData : BaseDataSO
{
    public string displayName;
    public string materials;
    public string counts;
    public string createItem;
    public string needObject;
}