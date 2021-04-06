using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Timers;
using UnityEngine.SceneManagement;
using System;

//1. Calls your Academy subclass's AcademyReset() function.
//2. Calls the AgentReset() function for each Agent in the scene.
//3. Calls the CollectObservations() function for each Agent in the scene.
//4. Uses each Agent's Policy to decide on the Agent's next action.
//5. Calls your subclass's AcademyStep() function.
//6. Calls the AgentAction() function for each Agent in the scene, passing in the action chosen by the Agent's Policy. (This function is not called if the Agent is done.)
//7. Calls the Agent's AgentOnDone() function if the Agent has reached its Max Step count or has otherwise marked itself as done. Optionally, you can set an Agent to restart if it finishes before the end of an episode. In this case, the Academy calls the AgentReset() function.


//public static class ExtensionsGameObject
//{
//    public static List<GameObject> GetAllChilds(this GameObject Go)
//    {
//        List<GameObject> list = new List<GameObject>();
//        for (int i = 0; i < Go.transform.childCount; i++)
//        {
//            list.Add(Go.transform.GetChild(i).gameObject);
//        }
//        return list;
//    }
//}

public class parkingAgent : Agent
{
    private Rigidbody rBody;
    public float speed = 10;
    public GameObject goal;
    private Timer timerStart;
    private Color m_GoalInitialColor;
    // is multipliert for Time.fixedDeltaTime (= usually 0.02 second) to get 20 Seconds
    private int m_timeTillSceneReset = 1000000;
    public Academy academy;
    public GameObject cars;
    public List<(Vector3, Quaternion)> carsInitialPositions;
    private DateTime StartTime;
    private float m_sensorLength = 10;
    private float[] distances;
    private int m_SensorNumber = 8;
    public float carSpeed = 0;
    private SimpleCarController1 simpleCarController1;
    private int m_RunCounter = 0;



    public float SensorLength
    {
        get
        {
            return m_sensorLength;
        }
        set
        {
            m_sensorLength = value;
        }
    }

    public int SensorNumber
    {
        get
        {
            return m_SensorNumber;
        }
        set
        {
            this.m_SensorNumber = value;
        }
    }

    public int RunCounter
    {
        get
        {
            return m_RunCounter;
        }
        set
        {
            m_RunCounter = value;
        }
    }







    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        simpleCarController1 = GetComponent<SimpleCarController1>();

        carsInitialPositions = this.GetAllChildsTransforms(cars);

        timerStart = new Timer(Time.fixedDeltaTime * m_timeTillSceneReset); // in ms -> 2000 = 2s
        timerStart.Elapsed += OnTimedEvent;
        timerStart.AutoReset = true;
        timerStart.Enabled = true;
        this.m_GoalInitialColor = goal.GetComponent<Renderer>().material.color;
        //this.m_agentRotation = new Quaternion(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
        //this.m_agentPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        //StartTime = DateTime.Now;

    }


    private void FixedUpdate()
    {
        this.carSpeed = this.rBody.velocity.x + this.rBody.velocity.z;
        ChangeColorWhenCarAtSpot();
        Sensor();
    }

    public void OnApplicationQuit()
    {
        timerStart.Stop();
        timerStart.Dispose();
    }
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("collision");
        AddReward(-0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("colliger triggered");
        AddReward(-0.1f);
    }










    // called each fixedUpdate
    // 1 step = 1 fixedUpdate
    // -> 1 step = 1 Action
    // is not affected by decision intervall
    // that means it 1 decision is made every X actions

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Debug.Log((DateTime.Now - StartTime).TotalSeconds);

        if (checkIfParked() && Moving())
        {
            SetReward(1.0f);
            Done();
        }
        else
        {
            AddReward(-0.005f);
        }

        // Actions, size = 2
        //Debug.Log("vertical: " + -vectorAction[0]);
        //Debug.Log("horizontal: " + vectorAction[1]);
        simpleCarController1.DriveRoutine(vectorAction[1],vectorAction[0]);
    }

    public override void CollectObservations()
    {
        // goal and Agent positions
        AddVectorObs(this.goal.transform.localPosition);
        AddVectorObs(this.transform.localPosition);

        // Agent velocity
        AddVectorObs(this.rBody.velocity.x);
        AddVectorObs(this.rBody.velocity.z);
        AddVectorObs(this.distances);
    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

    public override void AgentReset()
    {
        Debug.Log("RunCounter: " + this.RunCounter);
        RunCounter += 1;
        SetAllChildsTransforms(carsInitialPositions, cars);
        timerStart.Stop();
        timerStart.Start();
        this.transform.Rotate(new Vector3(0, 1, 0), UnityEngine.Random.Range(-180f, 180f));
        rBody.velocity = new Vector3(0f, 0f, 0f);
        simpleCarController1.CarReset();
        this.transform.localPosition = new Vector3(UnityEngine.Random.Range(-9f, 1.0f), 0f, UnityEngine.Random.Range(-3f, 3f)) + gameObject.transform.localPosition;
    }










    private List<(Vector3, Quaternion)> GetAllChildsTransforms(GameObject Go)
    {
        List<(Vector3, Quaternion)> list = new List<(Vector3, Quaternion)>();
        for (int i = 0; i < Go.transform.childCount; i++)
        {
            Transform tmp = Go.transform.GetChild(i).gameObject.transform;
            list.Add((new Vector3(tmp.localPosition.x, tmp.localPosition.y, tmp.localPosition.z), new Quaternion(tmp.rotation.x, tmp.rotation.y, tmp.rotation.z, tmp.rotation.w)));
        }
        return list;
    }

    private void SetAllChildsTransforms(List<(Vector3, Quaternion)> carsInitTransforms1, GameObject Go) // Go = GameObject
    {
        for (int i = 0; i < carsInitTransforms1.Count; i++)
        {
            Go.transform.GetChild(i).gameObject.transform.localPosition = carsInitTransforms1[i].Item1;
            Go.transform.GetChild(i).gameObject.transform.rotation = carsInitTransforms1[i].Item2;
        }
    }

    private void ChangeColorWhenCarAtSpot()
    {
        if (checkIfParked())
        {
            goal.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }
        else
        {
            goal.GetComponent<Renderer>().material.SetColor("_Color", m_GoalInitialColor);
        }
    }

    private bool Moving()
    {
        return Math.Abs(rBody.velocity.x) < 0.05 && Math.Abs(rBody.velocity.z) < 0.05;
    }




    private bool checkIfParked()
    {
        if (goal.transform.localPosition.x < this.transform.localPosition.x + 2.5f &&
            goal.transform.localPosition.x > this.transform.localPosition.x - 2.5f &&
            goal.transform.localPosition.z < this.transform.localPosition.z + 2.5f &&
            goal.transform.localPosition.z > this.transform.localPosition.z - 2.5f)
        {
            return true;
        }
        else
        {
            return false;

        }
    }

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        SetReward(-1.0f);
        Done();
    }

    private void Sensor()
    {
        float yOffSetTransform = 4f;
        Vector3 sensorStartPos = this.transform.localPosition;
        sensorStartPos.y += yOffSetTransform;
        // List<(raycastHit, rotation, hittet?)>
        List<(RaycastHit, Vector3, bool, float)> listRayCol = new List<(RaycastHit, Vector3, bool, float)>();

        (listRayCol, this.distances) = CreateListRayCol(listRayCol, sensorStartPos, this.SensorNumber);

        DrawForwardLine(listRayCol, sensorStartPos);


    }


    //int radius = 2;
    //var lengthToHit = Vector3.Distance(pos1 + Quaternion.AngleAxis(rotationDegree, this.transform.up) * this.transform.forward * radius, pos2);


    private (List<(RaycastHit, Vector3, bool, float)>, float[]) CreateListRayCol(List<(RaycastHit, Vector3, bool, float)> listRayCol, Vector3 sensorStartPosOffSet, int sensorCounter)
    {
        float[] distances = new float[8];
        float rotationPerSensor = 360f / sensorCounter;
        for (int i = 0; i < sensorCounter; i++)
        {

            //Quaternion currentRotationQuaternion = this.transform.rotation;
            //currentRotationQuaternion *= Quaternion.Euler(Vector3.up * 20);
            //bool hittet = Physics.Raycast(sensorStartPosOffSet, currentRotationQuaternion.eulerAngles, out hit, this.SensorLength);

            // radius for imaginary circle around car to decrease inaccuracy
            // (since normally ray's are starting from car's mid -> so min distance from car to obstacle is instead of 0 around 2-3 or something like that)
            //int radius = 2;
            RaycastHit hit;
            float currentRotation = i * rotationPerSensor;
            Vector3 currentRotationQuaternion = Quaternion.AngleAxis(currentRotation, this.transform.up) * this.transform.forward;
            bool hittet = Physics.Raycast(sensorStartPosOffSet, currentRotationQuaternion, out hit, this.SensorLength);
            listRayCol.Add((hit, currentRotationQuaternion, hittet, currentRotation));
            distances[i] = hittet ? hit.distance : Vector3.Distance(sensorStartPosOffSet, sensorStartPosOffSet + Quaternion.AngleAxis(currentRotation, this.transform.up) * this.transform.forward * this.SensorLength);
        }
        return (listRayCol, distances);
    }



    private void DrawForwardLine(List<(RaycastHit, Vector3, bool, float)> listRayCollision, Vector3 sensorStartPos)
    {
        foreach (var item in listRayCollision)
        {
            RaycastHit hit = item.Item1;
            Vector3 rotation = item.Item2;
            bool hittet = item.Item3;
            float rotation1 = item.Item4;
            //int radius = 2;

            if (hittet)
            {
                float lengthToHit = hit.distance;

                // with circle around car and not from mid of car
                //Debug.DrawLine(sensorStartPos + Quaternion.AngleAxis(rotation1, this.transform.up) * this.transform.forward * radius, sensorStartPos + Quaternion.AngleAxis(rotation1, this.transform.up) * this.transform.forward * this.SensorLength, Color.Lerp(Color.red, Color.white, lengthToHit / this.SensorLength));

                Debug.DrawLine(sensorStartPos, sensorStartPos + Quaternion.AngleAxis(rotation1, this.transform.up) * this.transform.forward * this.SensorLength, Color.Lerp(Color.red, Color.white, lengthToHit / this.SensorLength));


            }
            else
            {
                Debug.DrawLine(sensorStartPos, sensorStartPos + Quaternion.AngleAxis(rotation1, this.transform.up) * this.transform.forward * this.SensorLength, Color.white);
            }
        }
    }




}
