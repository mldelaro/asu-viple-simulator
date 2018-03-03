using UnityEngine;
using System.Collections;

public class HighlightMazeWallController : MonoBehaviour {

    private Color startColor;

    void OnMouseEnter()
    {
        startColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.blue;
    }

    void OnMouseDown()
    {
        Destroy(gameObject);
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = startColor;
    }
}
