using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    public float ChildMaxY;
    public bool IsFromEndPoint;
    public int BranchDepth;
    public Plant Plant;
    public Branch Parent;
    public Transform Origin;
    public Transform Endpoint;
    public List<JointTransform> Joints = new List<JointTransform>();
    public bool IsReversed;
    public class JointTransform
    {
        public Transform Transform;
        public bool IsReversed;
    }
    public List<Branch> Children = new List<Branch>();
    public List<Leaf> Leaves = new List<Leaf>();
    public int MaxChildren;
    public int Water;
    public int Food;

    [Range(0f, 1f)]
    public float BranchAngle;

    [Range(0f, 1f)]
    public float BranchLength;
    float lastBranchLength = -1;
    [Range(0f, 1f)]
    public float BranchGrowth;
    float lastBranchGrowth = -1;

    public Sprite[] BranchStateSprites;
    SpriteRenderer _sr;

    public void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    public int GetLeafAge()
    {
        return Mathf.FloorToInt((Plant.NumberOfLeafAges - 1) * BranchGrowth);
    }

    public int GetLeafHealth()
    {
        if (Water > 0) return 0;
        return Mathf.Abs(Mathf.FloorToInt(Water / Plant.TicksPerWaterStatus));
    }

    Quaternion jointRotRight = Quaternion.Euler(0, 0, -90);
    Quaternion jointRotLeft = Quaternion.Euler(0, 0, 90);

    public void GenerateJoints(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var reversed = Random.Range(0f, 1f) > 0.5;
            // Create join
            var joint = Instantiate(this.Plant.JointTemplate, Vector3.zero,
                reversed ? jointRotLeft : jointRotRight, this.transform);

            var pos = joint.transform.localPosition;
            pos.x = 0;
            pos.y = Random.Range(0, ChildMaxY);
            pos.z = 0;
            joint.transform.localPosition = pos;

            Joints.Add(new JointTransform() { Transform = joint.transform, IsReversed = reversed });
        }
    }

    public void GenerateLeaves(int count)
    {
        var reverse = Random.Range(0f, 1f) > 0.5;

        for (var i = 0; i < count; i++)
        {
            // Create join
            var leaf = (Leaf)Instantiate(this.Plant.LeafTemplate, Vector3.zero, Quaternion.identity, this.transform);

            var pos = leaf.transform.localPosition;
            pos.x = 0;
            pos.y = Random.Range(0, ChildMaxY);
            pos.z = 0;
            leaf.transform.localPosition = pos;

            if (reverse)
            {
                var scale = leaf.transform.localScale;
                scale.x = -1;
                leaf.transform.localScale = scale;
            }
            reverse = !reverse;

            leaf.Parent = this;
            Leaves.Add(leaf);
        }
    }

    public void UpdateBranch()
    {
        if (_sr == null) return;
        _sr.sortingOrder = BranchDepth * 2;

        // Move to origin
        transform.position = Origin.position;
        
        // Rotate branch
        var angle = IsFromEndPoint ? Mathf.Lerp(Plant.MinBranchAngle, Plant.MaxBranchAngle, BranchAngle) :
            Mathf.Lerp(Plant.MinJointAngle, Plant.MaxJointAngle, BranchAngle);
        var parentAngle = Origin.transform.rotation.eulerAngles.z;

        var rot = transform.localEulerAngles;
        rot.z = Mathf.Repeat(parentAngle + (angle * (IsReversed ? -1 : 1)), 360);
        transform.localEulerAngles = rot;

        // Scale branch
        if (lastBranchGrowth != BranchGrowth || lastBranchLength != BranchLength)
        {
            var growthScale = Mathf.Lerp(Plant.MinBranchGrowth, Plant.MaxBranchGrowth, BranchGrowth);
            var lengthScale = Mathf.Lerp(Plant.MinBranchLength, Plant.MaxBranchLength, BranchLength) * growthScale;
            var scale = this.transform.localScale;
            scale.x = growthScale;
            scale.y = lengthScale;
            transform.localScale = scale;
        }
        lastBranchGrowth = BranchGrowth;
        lastBranchLength = BranchLength;

        _sr.sprite = BranchStateSprites[GetLeafHealth()];

        for (var i = 0; i < Children.Count; i++)
            Children[i].UpdateBranch();
    }

    int GetParentFood()
    {
        return (this.Parent == null) ? this.Plant.Food : this.Parent.Food;
    }

    void SubtractParentFood(int amount)
    {
        if (this.Parent == null)
            this.Plant.Food -= amount;
        else
            this.Parent.Food -= amount;
    }

    public bool IsHealthy = true;

    public bool IsFullyGrown()
    {
        return BranchGrowth >= 1;
    }

    public bool HasMaxedOutChildren()
    {
        return this.Children.Count >= this.MaxChildren;
    }

    public void Tick()
    {
        IsHealthy = this.Parent == null || this.Parent.IsHealthy;

        // Pull water from resevoir (but only if child nodes are healthy)
        if (IsHealthy && Water < this.Plant.MaxBranchWater && this.Plant.Water > 0)
        {
            var waterFlow = Mathf.Clamp(this.Plant.WaterFlow, 0, this.Plant.Water);
            this.Plant.Water -= waterFlow;
            this.Water += waterFlow;
        }

        // Consume water (if none available, reduce health state)
        Water -= 1;
        if (Water < 0)
        {
            Water = Mathf.Clamp(Water, -1 * Plant.TicksPerWaterStatus * (Plant.NumberOfWaterStatuses - 1), 0);
            IsHealthy = false;
        }

        // Push food to children
        if (IsHealthy && IsFullyGrown())
        {
            var foodSplit = Mathf.CeilToInt(Plant.FoodFlow / (float)(Children.Count + (HasMaxedOutChildren() ? 0 : 1)));
            var foodFlow = Mathf.Clamp(Plant.FoodFlow, 0, Food);
            // Keep food back if still sprouting
            if (!HasMaxedOutChildren())
            {
                foodFlow -= foodSplit;
            }
            for (var i = 0; i < Children.Count; i++)
            {
                var childFlow = Mathf.Clamp(Mathf.Clamp(foodSplit, 0, foodFlow), 0, Plant.MaxBranchFood - Children[i].Food);
                Children[i].Food += childFlow;
                Food -= childFlow;
                foodFlow -= childFlow;
            }
        }

        // Attempt to grow
        if (IsHealthy && Food > 0 && !IsFullyGrown())
        {
            BranchGrowth += this.Plant.BranchGrowthPerTick;
            Food -=1;
            BranchGrowth = Mathf.Clamp(BranchGrowth, 0, 1);
        }

        // Attempt to sprout
        if (IsHealthy && IsFullyGrown() && Food >= this.Plant.BranchSproutFoodCost && !HasMaxedOutChildren())
        {
            Food -= this.Plant.BranchSproutFoodCost;
            var growPoint = Random.Range(-1, Joints.Count);
            var jointTransform = growPoint == -1 ? this.Endpoint : Joints[growPoint].Transform;
            var isReversed = growPoint == -1 ? false : Joints[growPoint].IsReversed;
            this.Plant.GenerateBranch(this, jointTransform, this.BranchDepth + 1, isReversed);
        }
    }
}
