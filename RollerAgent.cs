using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent
{

    public GameObject test;
    private GameObject wall;
    private int rando;
    private bool floor;
    private int score;
    Rigidbody rBody;

    private bool isScorring;

    private float before;
    private float after;
    private float distanceBetween;


    void Start()
    {
        wall = GameObject.Find("Wall");
        rBody = GetComponent<Rigidbody>();
        before = distance(this.transform.position, test.transform.position);
        isScorring = true;
    }

    void Update()
    {
        if (wall.transform.position.x > 6 || wall.transform.position.x < -6 || wall.transform.position.z > 6 || wall.transform.position.z < -6)
        {
            isScorring = true;
            rando = Random.Range(0, 4);
            switch (rando)
            {
                case 0:
                    wall.transform.position = new Vector3(0, 0.5f, -5.5f);
                    wall.transform.eulerAngles = new Vector3(wall.transform.eulerAngles.x,0,wall.transform.eulerAngles.z);
                    WallMovement.wallspeed = Random.Range(4f, 12f);
                    break;
                case 1:
                    wall.transform.position = new Vector3(-5.5f, 0.5f, 0);
                    wall.transform.eulerAngles = new Vector3(wall.transform.eulerAngles.x, 90, wall.transform.eulerAngles.z);
                    WallMovement.wallspeed = Random.Range(4f, 12f);
                    break;
                case 2:
                    wall.transform.position = new Vector3(0, 0.5f, 5.5f);
                    wall.transform.eulerAngles = new Vector3(wall.transform.eulerAngles.x, 180, wall.transform.eulerAngles.z);
                    WallMovement.wallspeed = Random.Range(4f, 12f);
                    break;
                case 3:
                    wall.transform.position = new Vector3(5.5f, 0.5f, 0);
                    wall.transform.eulerAngles = new Vector3(wall.transform.eulerAngles.x, 270, wall.transform.eulerAngles.z);
                    WallMovement.wallspeed = Random.Range(4f, 12f);
                    break;
            }
        }
        
    }

    public Transform Target;
    public override void AgentReset()
    {
        if (transform.position.x > 5 || transform.position.x < -5 || transform.position.z > 5 || transform.position.z < -5)
            {
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0);
        }

        // Move the target to a new spot
        /*
        Target.position = new Vector3(Random.value * 8 - 4,
                                      0.5f,
                                      Random.value * 8 - 4);*/
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            floor = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            floor = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" && this.transform.position.y < 1.5)
        {
            SetReward(-20f);
        }

        if (other.gameObject.tag != "Wall")
        {
            SetReward(25f);
        }
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(Target.position);
        AddVectorObs(this.transform.position);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
        AddVectorObs(rBody.velocity.y);

    }

    public float distance(Vector3 obj1, Vector3 obj2) {
        float d;
        float x = obj1.x - obj2.x;
        float y = obj1.y - obj2.y;
        float z = obj1.z - obj2.z;


        d = (x * x) + (y * y) + (z * z);
        d = Mathf.Sqrt(d);
            
        return d;
    }
    public float speed = 30;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        controlSignal.y = vectorAction[2];
        if (vectorAction[2] != 1)
        {
            rBody.AddForce(controlSignal * speed);

        }
        else
        {
            if (floor)
            {
                rBody.AddForce(controlSignal * (1f), ForceMode.Impulse);

            }

        }


        
        if(this.transform.position.x > test.transform.position.x + 1)
        {

            SetReward(-1f);

        }else if (this.transform.position.x < test.transform.position.x - 1)
        {

            SetReward(-1f);
        }
        else
        {

            SetReward(0.1f);
        }


        if (this.transform.position.z > test.transform.position.z + 1)
        {
            SetReward(-1f);

        }
        else if (this.transform.position.z < test.transform.position.z - 1)
        {
            SetReward(-1f);

        }
        else
        {

            SetReward(0.1f);
        }


        if (distance(this.transform.position,wall.transform.position) < 2)
        {
            SetReward(-1f);
        }
        else
        {
            SetReward(3f);
        }





        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.position);

        // Reached target
        if (transform.position.x > 5 || transform.position.x < -5 || transform.position.z > 5 || transform.position.z < -5)
        {
            score -= 1;
            isScorring = false;
            SetReward(-10.0f);
            Debug.Log(score);

            Done();
        }

        if (wall.transform.position.x > 6 || wall.transform.position.x < -6 || wall.transform.position.z > 6 || wall.transform.position.z < -6)
        {
            if (transform.position.x < 5 && transform.position.x > -5 && transform.position.z < 5 && transform.position.z > -5 && isScorring)
            {
                score += 1;

                SetReward(20.0f);
                Debug.Log(score);
            }


        }

    }

}
