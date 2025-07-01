using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class HunterController : Agent
{
    // Hunter Variables
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;

    // Environment Variables
    Material environmentMaterial;
    public GameObject environment;

    public GameObject prey;
    public AgentController classObject;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        environmentMaterial = environment.GetComponent<Renderer>().material;
    }

    public override void OnEpisodeBegin()
    {
        // Hunter starting position
        Vector3 spawnLocation = new Vector3(Random.Range(-4.5f, 3.5f), 0.25f, Random.Range(-1f, -9.5f)); // Starting position of the hunter

        bool distanceGood = classObject.CheckOverlap(prey.transform.localPosition, spawnLocation, 5f);

        while (!distanceGood)
        {
            spawnLocation = new Vector3(Random.Range(-4.5f, 3.5f), 0.25f, Random.Range(-1f, -9.5f)); // Random position
            distanceGood = classObject.CheckOverlap(prey.transform.localPosition, spawnLocation, 5f);
        }

        transform.localPosition = spawnLocation; // Set the hunter's position
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        // sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);

        // Vector3 velocity = new Vector3(moveX, 0f, moveZ);
        // velocity = velocity.normalized * Time.deltaTime * moveSpeed;

        // transform.localPosition += velocity;

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agent"))
        {
            AddReward(10f);
            classObject.AddReward(-13f); // Penalize the prey for being caught
            environmentMaterial.color = Color.yellow; // Change environment color to yellow when all pellets are collected
            classObject.EndEpisode();
            EndEpisode();
        }

        if (other.CompareTag("Wall"))
        {
            environmentMaterial.color = Color.red; // Change environment color to red on collision with wall
            AddReward(-15f);
            classObject.EndEpisode();
            EndEpisode();
        }
    }
}
