using UnityEngine;
using System.Collections;

public class FollowRobotCamController : MonoBehaviour {

    public GameObject objectToFollow;
    public GameObject mainObjectCamera;
    private Camera thisCamera;
    private Camera mainCamera;
    private bool isOnMainCam;

	// Use this for initialization
	void Start () {
        isOnMainCam = false;
        thisCamera = this.GetComponent<Camera>();
        mainCamera = mainObjectCamera.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(objectToFollow.transform.localPosition.x, 15.0f, objectToFollow.transform.localPosition.z);
        if (Input.GetKey(KeyCode.Tab) && isOnMainCam)
        {
            thisCamera.enabled = true;
            mainCamera.enabled = false;
            isOnMainCam = false;
        }
        else if (Input.GetKeyUp(KeyCode.Tab) && !isOnMainCam)
        {
            mainCamera.enabled = true;
            thisCamera.enabled = false;
            isOnMainCam = true;
        }
            
	}
}
