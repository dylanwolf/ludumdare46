using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundboard : MonoBehaviour
{
    public AudioClip Water;
    public AudioClip PlantFood;
    public static Soundboard Current;
    AudioSource player;

    public void Awake()
    {
        Current = this;
    }
    public void Start()
    {
        player = GetComponent<AudioSource>();
    }

    public static void PlayWater()
    {
        
        Current.player.PlayOneShot(Current.Water);
    }

    public static void PlayPlantFood()
    {
        Current.player.PlayOneShot(Current.PlantFood);
    }
}
