/*
 * @Filename - RobotController.cs
 * @Author - Matthew de la Rosa
 * @Date - October 2015
 */

using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Handle the game object representing the robot
/// </summary>
public class RobotController : MonoBehaviour {

    private const int ACTION_PROFILE_STOP = 0;
    private const int ACTION_PROFILE_FORWARD = 1;
    private const int ACTION_PROFILE_BACKWARD = 2;
    private const int ACTION_PROFILE_LEFT = 3;
    private const int ACTION_PROFILE_RIGHT = 4;
    private const int ACTION_PROFILE_TURN = 5;

    private int sensorUpdateTimeout = 300; // time to send updates about sensors

    private int currentControlIndex = 0;
    private float currentTurnDegreeProfile = 0f;

    private int currentAbsoluteAngle = 0; // angle at which the robot should be set to.

    private RobotBean robotBean;

    private static TcpListener tcpListener;
    private Thread tcpListenerThread;
    System.IO.Stream output;

    private DistanceSensorController distanceSensorController;
    private DistanceSensorController distanceSensorFrontController;
    //private TouchSensorController touchSensorController;

    private String incomingTcpMessage;
   

    /// <summary>
    /// Initialize robot by creating a bean reference and opening a listener
    /// on a TCP port
    /// </summary>
	void Start () {
	    // Create Bean objects
        robotBean = new RobotBean(1, "testRobot");

        // Get references to sensor game objects
        distanceSensorController = GameObject.FindGameObjectWithTag("distanceSensor").GetComponent<DistanceSensorController>();
        distanceSensorFrontController = GameObject.FindGameObjectWithTag("distanceSensorFront").GetComponent<DistanceSensorController>();

        // Get command line parameters
        String[] arguments = Environment.GetCommandLineArgs();
        int tcpPortInput = 1350;

        if (arguments.Length > 1)
        {
            try
            {
                tcpPortInput = Int32.Parse(arguments[1]);
            }
            catch (FormatException e)
            {
                tcpPortInput = 1350;
            }

        }
       
        // Start TCP Listeners
        IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        tcpListener = new TcpListener(ipAddress, tcpPortInput);
        tcpListener.Start();
        
        this.tcpListenerThread = new Thread(new ParameterizedThreadStart(startTcpListener));
        tcpListenerThread.Start();
	}

     /* TCP CONNECTION HANDLERS*/
    private void startTcpListener(object s)
    {
        StateObjClass StateObj = new StateObjClass();
        StateObj.TimerCanceled = false;

        System.Threading.TimerCallback TimerDelegate = new System.Threading.TimerCallback(sendSensorUpdateProfile);
        System.Threading.Timer TimerItem = new System.Threading.Timer(TimerDelegate, null, 0, 260);
        StateObj.TimerReference = TimerItem;

        while (true)
        {

            if (tcpListener.Pending())
            {
                IAsyncResult result = tcpListener.BeginAcceptTcpClient(TcpListenerCallback, tcpListener);
                result.AsyncWaitHandle.WaitOne(250);
            }
            else
            {
                Thread.Sleep(100);
            }
        }
    }

    private TcpClient tcpContext = null;

    private void TcpListenerCallback(IAsyncResult result)
    {
        // read incoming tcp message
        tcpContext = tcpListener.EndAcceptTcpClient(result);
        StreamReader tcpStreamReader = new StreamReader(tcpContext.GetStream());
        while (true)
        {
            
            incomingTcpMessage = tcpStreamReader.ReadLine().Trim();
            /* TODO: Support more than 2 servos & take in any actuators*/
            float[] servoValues = new float[2];
            int currentId = -1;

            bool willTurnWholeRobot = false;
            float degreesToTurnRobot = 0;

            /* Parse incoming tcp message from JSON */
            Newtonsoft.Json.JsonTextReader jsonTextReader = new Newtonsoft.Json.JsonTextReader(new StringReader(incomingTcpMessage));

            while (jsonTextReader.Read())
            {
                // READ SERVO INPUTS
                if (jsonTextReader.TokenType.Equals(Newtonsoft.Json.JsonToken.PropertyName) && jsonTextReader.Value.Equals("servos"))
                {
                    while (!jsonTextReader.TokenType.Equals(Newtonsoft.Json.JsonToken.EndArray))
                    {
                        // Get if speed value is a turn
                        if (jsonTextReader.TokenType.Equals(Newtonsoft.Json.JsonToken.PropertyName) && jsonTextReader.Value.Equals("isTurn"))
                        {
                            jsonTextReader.Read();
                            if (jsonTextReader.Value != null)
                            {
                                bool isTurn = bool.Parse(jsonTextReader.Value.ToString());
                                // servo id = -1; turn the robot at specified degree via servoSpeed
                                if (currentId == -1)
                                {
                                    if (isTurn)
                                    {
                                        willTurnWholeRobot = true;
                                    }
                                    else
                                    {
                                        willTurnWholeRobot = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            jsonTextReader.Read();
                        }

                        // Get ServoId
                        if (jsonTextReader.TokenType.Equals(Newtonsoft.Json.JsonToken.PropertyName) && jsonTextReader.Value.Equals("servoId"))
                        {
                            jsonTextReader.Read();
                            if (jsonTextReader.Value != null && jsonTextReader.TokenType.Equals(Newtonsoft.Json.JsonToken.Integer))
                            {
                                currentId = int.Parse(jsonTextReader.Value.ToString()) % 2; //  TODO: Fix action profile maping to 0 and 1 index
                            }
                        }
                        // Get ServoSpeed
                        if (jsonTextReader.TokenType.Equals(Newtonsoft.Json.JsonToken.PropertyName) && jsonTextReader.Value.Equals("servoSpeed"))
                        {
                            jsonTextReader.Read();
                            if (jsonTextReader.Value != null)
                            {
                                // servo id = -1; turn the robot at specified degree via servoSpeed
                                if (currentId == -1)
                                {
                                    degreesToTurnRobot = float.Parse(jsonTextReader.Value.ToString());
                                }
                                else
                                {
                                    float servoSpeed = float.Parse(jsonTextReader.Value.ToString());
                                    servoValues[currentId] += servoSpeed;
                                }
                            }
                        }

                    }// end servos array
                } // end servos property
            }// end json reader

            /*TODO: Map correct motor actions to proper servo values*/

            if (willTurnWholeRobot)
            {
                currentControlIndex = ACTION_PROFILE_TURN;
                currentTurnDegreeProfile = degreesToTurnRobot;
            }
            else if (degreesToTurnRobot > 0)
            {
                currentControlIndex = ACTION_PROFILE_FORWARD;
            }
            else if (degreesToTurnRobot < 0)
            {
                currentControlIndex = ACTION_PROFILE_BACKWARD;
            }
            else if (degreesToTurnRobot == 0)
            {
                currentControlIndex = ACTION_PROFILE_STOP;
            }
            else
            {
                // left side + | right side - | => GO RIGHT
                if (servoValues[0] > 0 && servoValues[1] <= 0)
                {
                    currentControlIndex = ACTION_PROFILE_RIGHT;
                }
                // left side - | right side + | => GO LEFT
                else if (servoValues[0] <= 0 && servoValues[1] > 0)
                {
                    currentControlIndex = ACTION_PROFILE_LEFT;
                }
                // left side + | right side + | => GO FORWARD
                else if (servoValues[0] > 0 && servoValues[1] > 0)
                {
                    currentControlIndex = ACTION_PROFILE_FORWARD;
                }
                // left side - | right side - | => GO BACKWARD
                else if (servoValues[0] < 0 && servoValues[1] < 0)
                {
                    currentControlIndex = ACTION_PROFILE_BACKWARD;
                }
            }


            /*
            * OPEN OUTPUT STREAM AND WRITE ROBOT STATUS
            */
            String output2 = robotBean.toJSON() + "\n";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(robotBean.toJSON().Trim() + "\n");
            output = tcpContext.GetStream();
            output.Write(buffer, 0, buffer.Length);
        }
    }

	// Update is called once per frame
	void Update () {
	    /*TODO: update robot according to specific motor actions vs. general action profile*/
        switch (currentControlIndex)
        {
            case ACTION_PROFILE_STOP:
                ActionProfile_Stop();
                break;
            case ACTION_PROFILE_FORWARD:
                ActionProfile_MoveForward();
                break;
            case ACTION_PROFILE_BACKWARD:
                ActionProfile_MoveBackward();
                break;
            case ACTION_PROFILE_LEFT:
                StartCoroutine("ActionProfile_TurnLeft", 90);
                currentControlIndex = 0;
                break;
            case ACTION_PROFILE_RIGHT:
                StartCoroutine("ActionProfile_TurnRight", 90);
                currentControlIndex = 0;
                break;
            case ACTION_PROFILE_TURN:
                StartCoroutine("ActionProfile_TurnDegrees", Mathf.RoundToInt(currentTurnDegreeProfile));
                currentControlIndex = 0;
                break;
            default:
                currentControlIndex = 0;
                break;
        }
        //sendSensorUpdateProfile();
	}

    private class StateObjClass
    {
        // Used to hold parameters for calls to TimerTask.
        //public int SomeValue;
        public System.Threading.Timer TimerReference;
        public bool TimerCanceled;
    }

    private void sendSensorUpdateProfile(System.Object state)
    {
        StateObjClass stateObjClass = (StateObjClass)state;
        if (tcpContext != null)
        {

            // Update Robot Sensor Profile
            robotBean.sensorProfile = new System.Collections.Generic.List<SensorBean>();
            robotBean.actionProfile = incomingTcpMessage;

            float mDistance = distanceSensorController.distance;
            SensorBean distanceSensor = new SensorBean(1, "distance", "Laser");
            distanceSensor.readout.Add(new SensorReadoutBean("m", "distance", mDistance.ToString()));

            float mDistanceFront = distanceSensorFrontController.distance;
            SensorBean distanceSensorFront = new SensorBean(2, "distance", "Laser");
            distanceSensorFront.readout.Add(new SensorReadoutBean("m", "distance", mDistanceFront.ToString()));

            robotBean.sensorProfile.Add(distanceSensor);
            robotBean.sensorProfile.Add(distanceSensorFront);

            String strRobot = Regex.Replace(robotBean.toJSON().Trim(), @"\s+", "");
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(strRobot + "\n");

            output.Write(buffer, 0, buffer.Length);
        }
        else
        {
            stateObjClass.TimerReference.Dispose();
        }
    }

    /*
     *  SIMPLE DRIVING ROBOT ACTION PROFILE
     */
    public float movementSpeed = 1.0f;
    public float angleRotationSpeed = 40.0f;
    public bool isRotating = false;

    void ActionProfile_Stop()
    {
        transform.Translate(new Vector3(0, 0, 0));
    }

    void ActionProfile_MoveForward()
    {
        if (!isRotating)
        {
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        }
    }
    void ActionProfile_MoveBackward()
    {
        if (!isRotating)
        {
            transform.Translate(Vector3.back * movementSpeed * Time.deltaTime);
        }   
    }

    IEnumerator ActionProfile_TurnDegrees(int degreesToTurn)
    {
        if(degreesToTurn > 0) {
            StartCoroutine("ActionProfile_TurnRight", Math.Abs(degreesToTurn));
            //ActionProfile_TurnLeft(Math.Abs(degreesToTurn));
        }
        else if (degreesToTurn < 0)
        {
            StartCoroutine("ActionProfile_TurnLeft", Math.Abs(degreesToTurn));
            //ActionProfile_TurnRight(Math.Abs(degreesToTurn));
        }
        yield return null;
    }

    IEnumerator ActionProfile_TurnLeft(int degreesToTurn)
    {
        currentAbsoluteAngle = currentAbsoluteAngle - degreesToTurn;
        if (!isRotating)
        {
            isRotating = true;
            int newAngle = Mathf.RoundToInt(transform.eulerAngles.y - degreesToTurn) % 360;
            float deltaAngle = 0f;
            float currentTransform = transform.eulerAngles.y;

            while (deltaAngle < Math.Abs(Math.Abs(degreesToTurn) - 0.5f))
            {
                float updatedEulAngley = Mathf.MoveTowards(transform.eulerAngles.y, newAngle, angleRotationSpeed * Time.deltaTime);
                deltaAngle += Math.Abs(updatedEulAngley - transform.eulerAngles.y) % 360;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, updatedEulAngley);
                yield return null;
            }
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, degree);
            isRotating = false;
        }
    }

    IEnumerator ActionProfile_TurnRight(int degreesToTurn)
    {
        currentAbsoluteAngle = currentAbsoluteAngle + degreesToTurn;
        if (!isRotating)
        {
            isRotating = true;
            int newAngle = Mathf.RoundToInt(transform.eulerAngles.y + degreesToTurn);
            float deltaAngle = 0f;
            float currentTransform = transform.eulerAngles.y;

            while (deltaAngle < Math.Abs(Math.Abs(degreesToTurn) - 0.5f))
            {
                float updatedEulAngley = Mathf.MoveTowards(transform.eulerAngles.y, newAngle, angleRotationSpeed * Time.deltaTime);
                deltaAngle += Math.Abs(updatedEulAngley - transform.eulerAngles.y) % 360;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, updatedEulAngley);
                yield return null;
            }
            isRotating = false;
        }
    }

    public void resetRobot()
    {
        currentAbsoluteAngle = 0;
        // stop the robot
        transform.Translate(new Vector3(0f, 0f, 0f));
        // reset position of robot
        transform.position = new Vector3(2.0f, 0.4f, 2.0f);
        // reset angle of robot
        transform.eulerAngles = new Vector3(0f, 0f, 0f);


        // reset robot variables
        currentControlIndex = 0;
        currentTurnDegreeProfile = 0f;

        //stop turning routines
        /*this.StopCoroutine("ActionProfile_TurnLeft");
        this.StopCoroutine("ActionProfile_TurnRight");
        this.StopCoroutine("ActionProfile_TurnDegrees");*/
    }

    public void readjustRobot()
    {
        if (!isRotating)
        {
            // stop the robot
            //transform.Translate(new Vector3(0f, 0f, 0f));
            // reset angle of robot
            transform.eulerAngles = new Vector3(0f, (float)currentAbsoluteAngle, 0f);
        }
        

        //stop turning routines
        /*this.StopCoroutine("ActionProfile_TurnLeft");
        this.StopCoroutine("ActionProfile_TurnRight");
        this.StopCoroutine("ActionProfile_TurnDegrees");*/
    }
}
