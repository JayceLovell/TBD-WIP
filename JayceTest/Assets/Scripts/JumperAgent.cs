using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class JumperAgent : Agent
{
    public Rigidbody rBody;
    public Transform Target;
    public float speed;
    public float distanceToTarget;
    public Vector3 StartPosition;
    //public bool AtTarget;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        speed = 10;
        StartPosition = this.gameObject.GetComponent<Transform>().position;
    }

    public override void AgentReset()
    {
        if(this.transform.position.y <0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = StartPosition;
        }
        //Target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    public override void CollectObservations()
    {
        AddVectorObs(Target.position);
        AddVectorObs(this.transform.position);

        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
    }
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        controlSignal.y = vectorAction[2];
        rBody.AddForce(controlSignal * speed);

        //Rewards
        distanceToTarget = Vector3.Distance(this.transform.position,Target.position);

        // Reached target 
        if (distanceToTarget < 2.0f) {
            SetReward(1.0f);
            Done();
        }else if (this.transform.position.y < 0 || distanceToTarget > 2.1f)
        {
            SetReward(-1.0f);
            Done();
        }
    }
}
