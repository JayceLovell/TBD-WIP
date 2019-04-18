using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMovement2 : MonoBehaviour
{
    public GameObject goal;
    public Transform origin;
    public float minSpeed = 5;
    public float maxSpeed = 5;
    public float totalDistanceTravel = 20;

    public Transform[] spawnPoints;
    
    public float DistanceTravelled = 0f;

    private Vector3 startPosition;

    private Vector3 _origin = Vector3.zero;

    private float _speed = 0;

    private Rigidbody rBody;

    void Start()
    {
        this._speed = minSpeed;
        rBody = GetComponent<Rigidbody>();
        this._origin = origin.transform.position;
        this._origin.y = this.transform.position.y;
        Reset();
    }
    
    void Update()
    {
        DistanceTravelled = Vector3.Distance(this.transform.position, this.startPosition);
        // Use World space to do translate
        transform.Translate(this.transform.forward * Time.deltaTime * this._speed, Space.World);
        if (this.DistanceTravelled > this.totalDistanceTravel)
        {
            this.Reset();
        }
    }

    public void Reset()
    {
        this.rBody.velocity = Vector3.zero;
        this.rBody.angularVelocity = Vector3.zero;
        // Set the start position to calculate the distance travelled
        this.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length - 1)].position;
        this.startPosition = this.transform.position;
        // Rotate the wall to face origin point(0, 0, 0)
        this.transform.forward = (this._origin - this.transform.position).normalized;

        goal.transform.position = this.transform.position;

        Vector3 goalPosition = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        goal.transform.position = goalPosition;
        //this._speed = Random.Range(minSpeed, maxSpeed);
    }
}
