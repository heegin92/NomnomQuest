using Ironcow.Synapse.Data;
using System;

[System.Serializable]
public partial class FoodRecipeData : BaseDataSO
{
    public string displayName;
    public string materials;
    public int counts;
    public string createItem;
    public string needObject;
}