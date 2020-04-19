using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyToPlant : MonoBehaviour
{
    Plant plant;

    void Awake()
    {
        plant = GetComponent<Plant>();
    }

    void OnMouseDown()
    {
        GameEngine.Current.ApplyTool(plant);
    }
}
