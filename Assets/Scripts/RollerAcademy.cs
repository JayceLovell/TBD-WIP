using UnityEngine;
using MLAgents;

public class RollerAcademy : Academy
{
    [Header("Specific to Roller")]
    public float agentRunSpeed;
    public float agentJumpHeight;

    [HideInInspector]
    //use ~3 to make things less floaty
    public float gravityMultiplier = 2.5f;
    [HideInInspector]
    public float agentJumpVelocity = 777;
    [HideInInspector]
    public float agentJumpVelocityMaxChange = 10;
    public float distanceBeforeJump = 5f;

    // Use this for initialization
    public override void InitializeAcademy()
    {
        Physics.gravity *= gravityMultiplier;
    }

}