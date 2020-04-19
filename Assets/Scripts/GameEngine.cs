using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEngine : MonoBehaviour
{
    public List<Plant> Plants = new List<Plant>();
    public Slider TickLengthSlider;
    public float TickLength;
    float lastTickLength;
    float timer;

    public int WaterAvailable;
    public int FoodAvailable;

    public int WateringCanAmount;
    public int FoodAmount;
    public int WateringCanRefillAmount;
    public int PlantFoodRefillAmount;
    public Tools SelectedTool;
    public static GameEngine Current;

    void Awake()
    {
        Current = this;
    }

    public enum Tools
    {
        WateringCan,
        PlantFood
    }

    public void SelectWateringCan()
    {
        SelectedTool = Tools.WateringCan;
        ApplyTool(Plants[0]);
    }

    public void SelectPlantFood()
    {
        SelectedTool = Tools.PlantFood;
        ApplyTool(Plants[0]);
    }

    public void ApplyTool(Plant plant)
    {
        if (SelectedTool == Tools.PlantFood && FoodAvailable > FoodAmount)
        {
            plant.Food += FoodAmount;
            FoodAvailable -= FoodAmount;
            Soundboard.PlayPlantFood();
        }
        else if (SelectedTool == Tools.WateringCan && WaterAvailable > WateringCanAmount)
        {
            plant.Water += WateringCanAmount;
            WaterAvailable -= WateringCanAmount;
            Soundboard.PlayWater();
        }
    }

    public int FoodInSystem = 0;

    void Update()
    {
        TickLength = TickLengthSlider.value;

        if (TickLength != lastTickLength && timer > TickLength)
            timer = TickLength;
        lastTickLength = TickLength;

        if (timer < TickLength)
            timer += Time.deltaTime;
        else
        {
            timer -= TickLength;
            WaterAvailable += WateringCanRefillAmount;
            FoodAvailable += PlantFoodRefillAmount;
            FoodInSystem = 0;
            foreach (var plant in Plants)
            {
                plant.Tick();
                FoodInSystem += plant.TotalFoodInPlant;
            }
        }
    }
}
