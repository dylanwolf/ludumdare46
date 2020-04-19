using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    public Branch Parent;
    SpriteRenderer _sr;
    public int NumberOfLeafHealthStates = 4;

    public Sprite[] LeafStateSprites;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _sr.sortingOrder = (this.Parent.BranchDepth * 2) + 1;
        try
        {
            _sr.sprite = LeafStateSprites[(Parent.GetLeafAge() * NumberOfLeafHealthStates)+Parent.GetLeafHealth()];        
        }
        catch {
            Debug.Log($"Failed to set sprite: ({Parent.GetLeafAge()} * {NumberOfLeafHealthStates}) + {Parent.GetLeafHealth()} = (Parent.GetLeafAge() * NumberOfLeafHealthStates)+Parent.GetLeafHealth()");
        }
    }
}
