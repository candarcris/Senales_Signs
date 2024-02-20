using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizerAttribute : PropertyAttribute
{
    public float min, max;

    public RandomizerAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}
