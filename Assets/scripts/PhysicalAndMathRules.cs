using System.Collections;
using UnityEngine;

public class PhysicalAndMathRules
{
    public static Vector3 getDrag(Vector3 velocityOfObject, float dragCoefficient, float airDensity, float area)
    {
        Vector3 dragResult = -velocityOfObject.normalized * 0.5f * dragCoefficient * airDensity * area * velocityOfObject.sqrMagnitude;
        return dragResult;
    }

    public static float GetWindPressure(float speedOfWind)
    { 
        return 0.00256f * Mathf.Pow(speedOfWind * 0.44704f, 2);
    }

    public static Vector3 GetWindForce(Vector3 velocity, float area, float dragCoefficient)
    {
        return area * GetWindPressure(velocity.magnitude) * dragCoefficient * velocity.normalized;
    }

 
}

 
