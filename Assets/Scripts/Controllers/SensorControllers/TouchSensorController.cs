using UnityEngine;
using System.Collections;

public class TouchSensorController : MonoBehaviour
{
    public bool isPressed = false;

    private Color startColor;

    void Start()
    {
        startColor = GetComponent<Renderer>().material.color;
    }

    void OnCollisionEnter (Collision col)
    {
        GetComponent<Renderer>().material.color = Color.red;
        isPressed = true;
    }

    void OnCollisionExit(Collision col)
    {
        GetComponent<Renderer>().material.color = startColor;
        isPressed = false;
    }
}
