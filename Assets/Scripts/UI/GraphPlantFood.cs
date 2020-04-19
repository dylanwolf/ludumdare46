using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphPlantFood : MonoBehaviour
{
    public Image GraphImage;
    public int MaxAmount;
    public Color FillColorUnusable;
    public Color FillColorUsable; 

    // Update is called once per frame
    void Update()
    {
        var pct = Mathf.Clamp(GameEngine.Current.FoodAvailable / (float)MaxAmount, 0f, 1f);
        GraphImage.fillAmount = pct;
        GraphImage.color = GameEngine.Current.FoodAvailable < GameEngine.Current.FoodAmount ? FillColorUnusable : FillColorUsable;
    }
}
