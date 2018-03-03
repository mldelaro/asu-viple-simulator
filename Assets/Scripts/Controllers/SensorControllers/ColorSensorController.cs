using UnityEngine;
using System.Collections;

public class ColorSensorController : MonoBehaviour {

    private Color colorDetected;

    private Ray ray;
    private LineRenderer lineRenderer;
    private RaycastHit raycastHit;

	// Use this for initialization
	void Start () {
        this.GetComponent<Camera>().enabled = false;

        lineRenderer = (LineRenderer)this.GetComponent<LineRenderer>() as LineRenderer;
        lineRenderer.SetVertexCount(2);
        lineRenderer.SetWidth(0.05f, 0.05f);
        lineRenderer.useWorldSpace = false;
	}
	
	// Update is called once per frame
	void Update () {
        ray = new Ray(this.GetComponent<Camera>().transform.position, this.GetComponent<Camera>().transform.forward);
        // distance sensor detects some object set distance
        if (Physics.Raycast(ray, out raycastHit, 100))
        {
            Component detectedSurface = raycastHit.collider.GetComponent<Renderer>();
            //if(detectedSurface == null || detectedSurface.)
            //lineRenderer.SetPosition(1, new Vector3(0.0f, 0.0f, raycastHit.distance * 18.0f));
        }

        // no object in front, set distance to 0
        else
        {
            //distance = 1000.0f;
            lineRenderer.SetPosition(1, new Vector3(0.0f, 0.0f, 0.0f));
        }
	}
}
