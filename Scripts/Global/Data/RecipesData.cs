using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static Define;

[Serializable]
public class RecipesData {
    public string templateid { get; private set; }
    public string descriptionid { get; private set; }
    public string sprite { get; private set; }

    public ItemData[] ingredientDatas { get; private set; }
    public ItemData[] anythingIngredientDatas { get; private set; }
    public FoodType foodType { get; private set; }
    public int recipePrice { get; private set; }
    public bool isRecipeUseOven { get; private set; }
    public int difficulty { get; private set; }
    public Sprite recipeSprite { get; private set; }
    
    public RecipesData(string templateid, string descriptionid, string sprite, ItemData[] ingredientDatas, ItemData[] anythingIngredientDatas, FoodType foodType, int recipePrice, bool isRecipeUseOven = false)
    {
        this.templateid = templateid;
        this.descriptionid = descriptionid;
        this.sprite = sprite;
        this.ingredientDatas = ingredientDatas;
        this.anythingIngredientDatas = anythingIngredientDatas;
        this.foodType = foodType;
        this.recipePrice = recipePrice;
        this.isRecipeUseOven = isRecipeUseOven;

        int difficultyValue = isRecipeUseOven ? 1 : 0;
        foreach (var ingredient in ingredientDatas.Concat(anythingIngredientDatas)) {
            if (ingredient.difficulty != null) {
                difficultyValue += int.Parse(ingredient.difficulty);
            }
        }
        difficulty = difficultyValue;
        
        Managers.Resource.LoadAsync<Sprite>(sprite, callback: (recipeImg) => {
            recipeSprite = recipeImg;
        });
    }
    
    public void SetPrice(int price)
    {
        recipePrice = price;
    }
    
    public void SetOvenState(bool state)
    {
        isRecipeUseOven = state;
    }
}