using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent
{
    public RollerAcademy academy;
    public float JumpForce = 5.0f;
    public float JumpTime = 2.0f;
    // Wall to dodge
    public WallMovement wall;

    public GameObject goal;
    // Rigidbody of Agent and Wall
    Rigidbody agentRB;
    Rigidbody wallRB;

    public float fallingForce;
    public float maximumJumpDistance;

    private Vector3 startPosition;
    
    private Vector3 jumpStartingPos;

    private float _jumpingElapsed = 0;



    void Start()
    {
        academy = FindObjectOfType<RollerAcademy>();
        agentRB = GetComponent<Rigidbody>();
        wallRB = wall.GetComponent<Rigidbody>();
        startPosition = this.transform.position;
    }
    
    public override void AgentReset()
    {
        // Reset wall position
        wall.Reset();
        this.agentRB.angularVelocity = Vector3.zero;
        this.agentRB.velocity = Vector3.zero;
        this.transform.position = startPosition;
        this.transform.forward = -wall.transform.forward;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            this.Done();
            this.SetReward(-1.0f);
        }
    }
    // TODO Add distance of y to observation and add reward if distance is smaller
    /// <summary>
    /// Collect data for the brain
    /// </summary>
    public override void CollectObservations()
    {
        // Space size for vector observation is total float values collected. Vector3 has 3 float values
        // Agent Postion
        AddVectorObs(this.transform.position);
        // Wall Position
        AddVectorObs(wall.transform.position);
        // Distance from the wall, the intention of this is to make sure agent keep a distance from the wall, it should roll back it wall is distanceBeforeJump away
        // It is recommand to have -1 to 1 or 0 to 1 as the value
        AddVectorObs(DistanceFromWall() < academy.distanceBeforeJump ? 1 : 0);
        // Jump Force so it does not jump too high
        AddVectorObs(agentRB.velocity.y);
        // Velocity of the wall
        AddVectorObs(wallRB.velocity.x);
        AddVectorObs(wallRB.velocity.z);
        //  float gap = this.transform.position.y - 0.5f - wall.transform.position.y + 0.5f;
        // Gap between bottom of agent and top of the wall
        // AddVectorObs(gap >= 0 && gap < 0.5f ? 1 : 0);
        // Facing Wall and is in air when wall is 3 unit infront
        AddVectorObs(Grounded() ? 1 : 0);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        MoveAgent(vectorAction);
        
        if (PushedOut() || !InsideArea())
        {
            this.Done();
            this.SetReward(-1f);
        }
    }

    public void MoveAgent(float[] act)
    {
        // Make the agent do the task as quick as possible
        this.AddReward(-0.001f);
        // Discrete Space Type has Number Of Input + 1 size
        // act[0] is the current action and by default have value 0
        Vector3 movingDirection = Vector3.zero;
        // For discrete vector action space
        // act.length = branch size
        // Each branch has a size which is integer from 0 to the size of the branch.
        // For example, movement can have size of 3, 0 = no movement, 1 = move forward, 2 = move backward
        // Each discrete input has a branch index which link the value of the inputs
        int forwardAction = (int)act[0];
        int jumpAction = (int)act[1];

        if (forwardAction == 1)
        {
            movingDirection = transform.forward;
        }
        else if(forwardAction == 2)
        {
            movingDirection = -transform.forward;
        }
        agentRB.AddForce(movingDirection * academy.agentRunSpeed, ForceMode.VelocityChange);

        if (jumpAction == 1)
        {
            if (this._jumpingElapsed <= 0)
            {
                if(DistanceFromWall() < academy.distanceBeforeJump)
                {
                    this.AddReward(0.1f);
                }
                Jump();
            }
        }

        /* Punishment */
        // If jump too early
        if (DistanceFromWall() > academy.distanceBeforeJump && agentRB.velocity.y > 0)
        {
            this.AddReward(-0.1f);
        }

        // Check if agent is maximumJumpDistance units above wall
        if ((this.transform.position.y - wall.transform.position.y) > maximumJumpDistance)
        {
            this.AddReward(-0.1f);
        }

        /* Physics */
        // Apply upward force if jumping
        if (this._jumpingElapsed > 0)
        {
            Vector3 jumpTargetPos = new Vector3(transform.position.x, jumpStartingPos.y + academy.agentJumpHeight, transform.position.z);
            agentRB.velocity = Vector3.MoveTowards(agentRB.velocity, jumpTargetPos, academy.agentJumpVelocityMaxChange);
        }

        // Apply downward force if jumping time is over and not grounded
        if (!(this._jumpingElapsed > 0f) && !Grounded())
        {
            agentRB.AddForce(Vector3.down * this.fallingForce, ForceMode.Acceleration);
        }

        
        this._jumpingElapsed -= Time.deltaTime;
    }

    // Detect when the agent hits the goal
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("goal") && Grounded())
        {
            SetReward(1f);
            Done();
        }
    }

    public float DistanceFromWall()
    {
        return Vector3.Distance(this.transform.position, wall.transform.position);
    }

    public bool IsFacingWall()
    {
        return Vector3.Dot((wall.transform.position - transform.position).normalized, transform.forward) > 0;
    }

    public bool PushedOut()
    {
        return this.transform.position.y < 0;
    }

    public void MoveTowards()
    {
        agentRB.velocity = Vector3.MoveTowards(agentRB.transform.position, agentRB.transform.position + Vector3.up * JumpForce, 10);
    }

    public void Jump()
    {
        this._jumpingElapsed = this.JumpTime;
       jumpStartingPos = transform.position;
    }

    public bool Grounded()
    {
        RaycastHit hit;
        Physics.Raycast(this.transform.position, Vector3.down, out hit, 1f);
        if (hit.collider != null && (hit.collider.CompareTag("floor") || hit.collider.CompareTag("goal")))
        {
            return true;
        }
        return false;
    }

    public bool InsideArea()
    {

        RaycastHit hit;
        Physics.Raycast(this.transform.position, Vector3.down, out hit, 20f);
        if (hit.collider != null && (hit.collider.CompareTag("floor") || hit.collider.CompareTag("goal") || hit.collider.CompareTag("wall")))
        {
            return true;
        }
        return false;
    }

}
