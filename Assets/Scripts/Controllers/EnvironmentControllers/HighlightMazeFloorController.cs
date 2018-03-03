using UnityEngine;
using System.Collections;

public class HighlightMazeFloorController : MonoBehaviour {
    
    private Color startColor = Color.white;
    private bool isPainted = false;
    public GameObject spawnCube_MazeWall;

    void OnMouseEnter()
    {
        startColor = GetComponent<Renderer>().material.color;
    }

    void OnMouseOver()
    {
        // maze floor already painted, remove paint if right click
        if (isPainted)
        {
            if (Input.GetMouseButtonDown(1))
            {
                ErasePaintFromMazeFloor();
            }
            return;
        }

        // floor is not painted
        else
        {
            GetComponent<Renderer>().material.color = Color.blue;

            //left click
            if (Input.GetMouseButtonDown(0))
            {
                SpawnMazeWall();
            }

            // right click
            else if (Input.GetMouseButtonDown(1))
            {
                PaintMazeFloor();
            }
        }
    }

    private void SpawnMazeWall()
    {
        GetComponent<Renderer>().material.color = Color.green;
        float spawnPosX = GetComponent<Transform>().position.x;
        float spawnPosZ = GetComponent<Transform>().position.z;
        float spawnPosY = 0.5f;
        Vector3 newWallPosition = new Vector3(spawnPosX,
                                                spawnPosY,
                                                spawnPosZ);
        Instantiate(spawnCube_MazeWall, newWallPosition, Quaternion.identity);
    }

    private void PaintMazeFloor()
    {
        GetComponent<Renderer>().material.color = Color.red;
        isPainted = true;
    }

    private void ErasePaintFromMazeFloor()
    {
        GetComponent<Renderer>().material.color = Color.white;
        isPainted = false;
    }

    void OnMouseExit()
    {
        if (!isPainted)
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
