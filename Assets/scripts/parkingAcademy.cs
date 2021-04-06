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



public class parkingAcademy : Academy {


    // private int test = 0;


    // Use this for initialization
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{
    //    //test++;
    //    //if (test > 10)
    //    //{
    //    //    this.AcademyReset();
    //    //    test = 0;
    //    //}
    //}

    //public override void AcademyReset()
    //{
        //Debug.Log("AcademyRessssssssset");
    //}

}