using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public string recipeName;
    public List<Item> requiredItems;
    public Item resultItem;
}

public class RecipeManager : MonoBehaviour
{
    public List<Recipe> recipes;

    public Item Cook(List<Item> ingredients)
    {
        foreach (var recipe in recipes)
        {
            bool match = true;
            foreach (var req in recipe.requiredItems)
            {
                if (!ingredients.Exists(i => i.itemName == req.itemName && i.quantity >= req.quantity))
                {
                    match = false;
                    break;
                }
            }
            if (match) return recipe.resultItem;
        }
        return null; // ½ÇÆÐ ½Ã
    }
}
