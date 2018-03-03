using UnityEngine;
using System.Collections;

public class SpawnMazeFloorController : MonoBehaviour
{

    public Transform gridStartPoint;
    public GameObject spawnPlane_MazeFloor;
    public GameObject spawnCube_MazeWall;
    public Object[][] mazeFloor;
    public const int row = 14;
    public const int col = 22;
    public bool[,] mazeLayout = new bool[row, col] {    { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true },
                                                        { true, false, false, false, true, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, true },
                                                        { true, false, false, false, true, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false ,true },
                                                        { true, false, false, false, true, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false ,true },
                                                        { true, false, false, false, true, true, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true },
                                                        { true, false, false, false, true, true, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true },
                                                        { true, false, false, false, true, true, false, false, true, true, false, false, true, true, false, false, false, false, false, false, false, true },
                                                        { true, false, false, false, true, true, false, false, true, true, false, false, true, true, false, false, false, false, false, false, false, true },
                                                        { true, false, false, false, true, true, false, false, true, true, false, false, true, true, true, true, true, true, false, false, false, true },
                                                        { true, false, false, false, true, true, false, false, true, true, false, false, true, true, true, true, true, true, false, false, false, true },
                                                        { true, false, false, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, true},
                                                        { true, false, false, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, true},
                                                        { true, true,  true, true,  true, true, true, true, true, true, true, true, true,  true, true,  true, true, true, true, true, true, true },
                                                        { true, true,  true, true,  true, true, true, true, true, true, true, true, true,  true, true,  true, true, true, true, true, true, true }};

    // Use this for initialization
    void Start()
    {
        //spawn all floors
        for (int r = row; r > 0; r--)
        {
            for (int c = col; c > 0; c--)
            {
                Vector3 newFloorPosition = new Vector3(gridStartPoint.transform.localPosition.x + c,
                                                gridStartPoint.transform.localPosition.y + 0,
                                                gridStartPoint.transform.localPosition.z + r);

                Object newFloor = Instantiate(spawnPlane_MazeFloor, newFloorPosition, Quaternion.identity);

                if (mazeLayout[r-1, c-1])
                {
                    float spawnPosX = newFloorPosition.x;
                    float spawnPosZ = newFloorPosition.z;
                    float spawnPosY = 0.5f;
                    Vector3 newWallPosition = new Vector3(spawnPosX,
                                                            spawnPosY,
                                                            spawnPosZ);
                    Instantiate(spawnCube_MazeWall, newWallPosition, Quaternion.identity);
                }
            }
        }
    }
}
