using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Math2D
{
    public static Vector2 GetClosestPointOnLineSegment(Vector2 A, Vector2 B, Vector2 P, out float distanceToP)
    {
        Vector2 AP = P - A;       //Vector from A to P   
        Vector2 AB = B - A;       //Vector from A to B  

        float magnitudeAB = AB.SqrMagnitude();//.LengthSquared();     //Magnitude of AB vector (it's length squared)     
        float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
        float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

        Vector2 value;
        if (distance < 0)     //Check if P projection is over vectorAB     
        {
            value = A;

        }
        else if (distance > 1)
        {
            value = B;
        }
        else
        {
            value = A + AB * distance;
        }
        distanceToP = Vector2.Distance(P, value);

        return value;
    }

}
