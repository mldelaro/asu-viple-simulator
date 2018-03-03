using UnityEngine;
using System.Collections;

public class GuiManagerController : MonoBehaviour {

    public RobotController robotToReset;

   public  void onClickResetButton()
    {
        robotToReset.resetRobot();
    }

   public void onClickReadjustButton()
   {
       robotToReset.readjustRobot();
   }
}
