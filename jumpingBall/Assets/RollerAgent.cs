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
    private float wallSpeed = 4f;
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
			rando = 1;
            switch (rando)
            {
                case 0:
                    wall.transform.position = new Vector3(0, 0.5f, -5.5f);
                    wall.transform.eulerAngles = new Vector3(wall.transform.eulerAngles.x,0,wall.transform.eulerAngles.z);
                    
                    break;
                case 1:
                    wall.transform.position = new Vector3(-5.5f, 0.5f, 0);
                    wall.transform.eulerAngles = new Vector3(wall.transform.eulerAngles.x, 90, wall.transform.eulerAngles.z);
                    break;
                case 2:
                    wall.transform.position = new Vector3(0, 0.5f, 5.5f);
                    wall.transform.eulerAngles = new Vector3(wall.transform.eulerAngles.x, 180, wall.transform.eulerAngles.z);
                    break;
                case 3:
                    wall.transform.position = new Vector3(5.5f, 0.5f, 0);
                    wall.transform.eulerAngles = new Vector3(wall.transform.eulerAngles.x, 270, wall.transform.eulerAngles.z);

                    break;
            }
            WallMovement.wallspeed = wallSpeed;
        }
		if (Input.GetKeyDown("1"))
		{
			wallSpeed = 4f;
			Debug.Log(wallSpeed);
			Debug.Log("level1");
		}

		if (Input.GetKeyDown("2"))
		{
			wallSpeed = 8f;
			Debug.Log(wallSpeed);

			Debug.Log("level2");
		}
		if (Input.GetKeyDown("3"))
		{
			wallSpeed = 12f;
			Debug.Log(wallSpeed);
	
			Debug.Log("level3");
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
    {/*
        if (other.gameObject.tag == "Wall" && this.transform.position.y < 1.2f)
        {
            AddReward(-0.3f);
        }*/

        //if (other.gameObject.tag != "Wall")
        //{
        //    SetReward(0.2f);
        //}
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        //AddVectorObs(Target.position);
        AddVectorObs(this.transform.position);
        AddVectorObs(wall.transform.position);
        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
        AddVectorObs(rBody.velocity.y);
        AddVectorObs(wall.GetComponent<Rigidbody>().velocity.x);
        AddVectorObs(wall.GetComponent<Rigidbody>().velocity.z);
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
    public float speed = 10;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        controlSignal.y = vectorAction[2];
        /*
        if (vectorAction[2] != 1)
        {
            rBody.AddForce(controlSignal * speed);

        }
        else if (floor)
        {
  
           rBody.AddForce(new Vector3(0,0.4f,0), ForceMode.Impulse);

        }
        */
        rBody.AddForce(new Vector3(controlSignal.x*3f, 0, controlSignal.z * 3f)  * speed);
        if (vectorAction[2] > 0f&&floor)
        {

            rBody.AddForce(new Vector3(0, 60f, 0), ForceMode.Impulse);

        }


        //if (this.transform.position.x > test.transform.position.x + 1)
        //{

        //    SetReward(-0.1f);

        //}
        //else if (this.transform.position.x < test.transform.position.x - 1)
        //{

        //    SetReward(-0.1f);
        //}
        //else
        //{

        //    SetReward(0.1f);
        //}


        //if (this.transform.position.z > test.transform.position.z + 1)
        //{
        //    SetReward(-0.1f);

        //}
        //else if (this.transform.position.z < test.transform.position.z - 1)
        //{
        //    SetReward(-0.1f);

        //}
        //else
        //{

        //    SetReward(0.2f);
        //}


        //if (distance(this.transform.position, wall.transform.position) < 1.5 && floor)
        //{
        //    SetReward(-0.8f);
        //}
        //else
        //{
        //    SetReward(0.2f);
        //}
        
 



        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  test.transform.position);

		//if (distanceToTarget < 1)
		//{
		//	AddReward(0.00001f);

		//}
		//else
		//{
		//	AddReward(-0.00001f);

		//}


		if (transform.position.x > 5 || transform.position.x < -5 || transform.position.z > 5 || transform.position.z < -5)
        {
            score -= 1;
            isScorring = false;
            SetReward(-1f);
            Debug.Log(score);
			Done();

		}

		if (wall.transform.position.x > 6 || wall.transform.position.x < -6 || wall.transform.position.z > 6 || wall.transform.position.z < -6)
        {
            if (transform.position.x < 5 && transform.position.x > -5 && transform.position.z < 5 && transform.position.z > -5 && isScorring)
            {
                score += 1;

                SetReward(1f);
                Done();
                Debug.Log(score);
            }


        }

    }

}
