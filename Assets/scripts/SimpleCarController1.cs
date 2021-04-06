using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class SimpleCarController1 : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public Transform transformFrontLeft;
    public Transform transformFrontRight;
    public Transform transformRearLeft;
    public Transform transformRearRight;


    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider, Transform transform)
    {

        Vector3 position;
        Quaternion rotation;
        
        // does work but maybe not with multiple areas
        //collider.GetWorldPose(out position, out rotation);
        //transform.transform.position = position;


        //transform.rotation = ;
    }

    public void FixedUpdate()
    {

        DriveRoutine(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    }


    public void DriveRoutine(float verticalInput, float horizontalInput)
    {
        float motor = maxMotorTorque * verticalInput;
        float steering = maxSteeringAngle * horizontalInput;

        foreach (AxleInfo axleInfo in axleInfos)
        {
            // stop vibrating/jitting of car
            axleInfo.leftWheel.ConfigureVehicleSubsteps(5f, 100, 15);
            axleInfo.rightWheel.ConfigureVehicleSubsteps(5f, 100, 15);


            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
        ApplyLocalPositionToVisuals(axleInfos[0].leftWheel, transformFrontLeft);
        ApplyLocalPositionToVisuals(axleInfos[0].rightWheel, transformFrontRight);
        ApplyLocalPositionToVisuals(axleInfos[1].leftWheel, transformRearLeft);
        ApplyLocalPositionToVisuals(axleInfos[1].rightWheel, transformRearRight);

    }

    public void CarReset()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            axleInfo.leftWheel.steerAngle = 0;
            axleInfo.rightWheel.steerAngle = 0;
            axleInfo.leftWheel.motorTorque = 0;
            axleInfo.rightWheel.motorTorque = 0;
        }
    }
}