using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float MinBranchLength;
    public float MaxBranchLength;
    public float MinBranchGrowth;
    public float MaxBranchGrowth;
    public float MinBranchAngle;
    public float MaxBranchAngle;
    public float MinJointAngle;
    public float MaxJointAngle;
    public int MinBranchJoints;
    public int MaxBranchJoints;
    public int MinBranchLeaves;
    public int MaxBranchLeaves;
    public int MinBranchChildren;
    public int MaxBranchChildren;
    public Branch Child;

    [System.NonSerialized]
    public Transform PlantOrigin;

    public Branch BranchTemplate;
    public Leaf LeafTemplate;
    public GameObject JointTemplate;
    public List<List<Branch>> ChildrenByDepth = new List<List<Branch>>();

    public void GenerateBranch(Branch parent, Transform joint, int depth)
    {
        if (
            (parent == null && this.Child != null) ||
            (parent != null && parent.Children.Count >= parent.MaxChildren)
        ) return;

        var b = Instantiate(BranchTemplate, joint.transform.position, Quaternion.identity, joint);
        b.Plant = this;
        b.Parent = parent;
        b.Origin = joint;
        b.IsFromEndPoint = (parent == null && joint == PlantOrigin) || (parent != null && joint == parent.Endpoint);
        b.BranchAngle = Random.Range(0.0f, 1.0f);
        b.BranchGrowth = 0;
        b.BranchLength = 0;
        b.BranchDepth = depth;
        b.GenerateJoints(Random.Range(MinBranchJoints, MaxBranchJoints+1));
        b.GenerateLeaves(Random.Range(MinBranchLeaves, MaxBranchLeaves+1));
        b.MaxChildren = Random.Range(MinBranchChildren, MaxBranchChildren+1);

        b.transform.localScale = Vector3.zero;

        while (ChildrenByDepth.Count <= depth)
        {
            ChildrenByDepth.Add(new List<Branch>());
        }
        ChildrenByDepth[depth].Add(b);

        if (parent != null) parent.Children.Add(b);
        else this.Child = b;
    }

    public void Start()
    {
        PlantOrigin = this.transform;
        this.GenerateBranch(null, this.PlantOrigin, 0);
    }

    public float BranchGrowthPerTick;
    public int WaterFlow;
    public int FoodFlow;
    public int Food;
    public int Water;
    public int MaxBranchWater;
    public int MaxBranchFood;
    public int BranchSproutFoodCost;
    public int NumberOfWaterStatuses;
    public int TicksPerWaterStatus;
    public int NumberOfLeafAges;
    public int TotalFoodInPlant = 0;

    public void Tick()
    {
        // Push food to immediate child
        var foodFlow = Mathf.Clamp(FoodFlow, 0, Food);
        Child.Food += foodFlow;
        Food -= foodFlow;

        // Process children
        TotalFoodInPlant = 0;
        var originalDepth = ChildrenByDepth.Count;
        for (var d = 0; d < originalDepth; d++)
        {
            var originalLength = ChildrenByDepth[d].Count;
            for (var i = 0; i < originalLength; i++)
            {
                ChildrenByDepth[d][i].Tick();
                TotalFoodInPlant += ChildrenByDepth[d][i].Food;
            }
        }
    }
}
