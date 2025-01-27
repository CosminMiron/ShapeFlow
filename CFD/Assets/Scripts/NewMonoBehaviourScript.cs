using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    void Update()
    {
        var maxAngle = 20;
                
                           // Check the object's current rotation in world space
        Vector3 rotation = transform.eulerAngles;
 
        // Normalize angles to [-180, 180] for easier comparison
        float normalizedX = NormalizeAngle(rotation.x);
        float normalizedZ = NormalizeAngle(180 - rotation.y);
 
        // Calculate the tilt angle (distance from horizontal)
        float tiltAngle = Mathf.Max(Mathf.Abs(normalizedX), Mathf.Abs(normalizedZ));
 
        // Map the tilt angle to a gradient value (0 = green, 1 = red)
        float t = Mathf.Clamp01(tiltAngle / maxAngle);
 
        // Interpolate between green and red based on the tilt
Color gradientColor = Color.Lerp(Color.green, Color.red, t);
 
        // Apply the color to the object
        gameObject.GetComponent<Renderer>().material.color = gradientColor;
    }

        // Function to normalize angles to the range [-180, 180]
    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
 
}
