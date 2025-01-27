public class Define
{
    public const string RECIPE = "recipe";
    public const string COIN = "coin";
    public const string CASH = "cash";
    public const string STORY = "story";
    
    public const string STOREREPUTATION = "storereputation";
    public const string TUTORIAL = "tutorial";
    
    public const string FARMING = "farming";
    public const string DELIVERY = "delivery";
    public const string COUNTER = "counter";
    public const string SERVING = "serving";
    public const string MAKINGFOOD = "makingfood";
    public const string OVEN = "oven";
    public const string MENU = "menu";
    
    public const string TRADE = "trade";
    public const string PULLING = "pulling";
    public const string ACHIEVEMENT = "achievement";
    public const string HAVING = "having";
    public const string INGREDIENT = "ingredient";
    public const string PICNICBOX = "picnicbox";
    public const string LOCATION = "location";
    
    public enum UIEvent
    {
        Click,
        Press
    }

    public enum SceneType
    {
        Unknown,
        DevScene,
        CutScene,
        MainScene,
        TitleScene,
    }

    public enum IngredientType {
        Default,
        Fruit,
        Vegetable,
        Sauce,
        Other,
        Special,
        Nuts,
    }

    public enum FoodType
    {
        Default,
        Dessert,
        Brunch,
        Ocean,
        DarkForest,
        Drink,
        UnderCave,
        Volcano,
        SkyLand
    }

    public enum LocationType
    {
        Default,
        Oven,
        Hall,
        UnderCave,
        Ocean,
        DarkForest,
        Volcano,
        SkyLand
    }

    public enum DialogType
    {
        Order,
        Correct,
        Wrong,
        TimeOver
    }

    public enum ReviewType
    {
        level1,
        level2,
        level3,
        level4,
        level5,
    }

    public enum InteriorType
    {
        Chair,
        Window,
        Light,
        Object,
        Wallpaper,
        Floor
    }
    
    public enum Tutorials {
        MainTutorial,
        TakeOrderCustomer,
        HowToMakingFood,
        HowToDragFood1,
        HowToDragFood2,
        HowToDragFood3,
        HowToDeliveryFood1,
        HowToDeliveryFood2,
        HowToDeliveryFood3,
        HowToDeliveryFood4,
        HowToDeliveryFood5,
        HowToDeliveryFood6,
        HowToDeliveryFood7,
        MainTutorialEnd,
        FarmTutorial1,
        FarmTutorial2,
        FarmTutorial3,
        FarmTutorialEnd,
        PlayingFarmTutorial,
        PicnicBoxTutorial1,
        PicnicBoxTutorial2,
    }
    
    public enum VersatileType
    {
        UsingFarmBoost,
        UsingDeliveryBoost,
        BuyFarm,
        Alert
    }
    
    /*
     * Achievement
     */
    public enum AchievementType
    {
        story,
        tutorial,
        storereputation,
        farming,
        serving,
        achievement,
        oven,
        having
    }

    public enum Character {
        Moss,
    }

    public enum PlayerState {
        StoreReputation,
        TotalFarmingCount,
        TotalServingCount,
        TotalOvenCount,
        TotalOrderCount,
    }
}
