using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using Unity.VisualScripting;
using System.Collections.Generic;

public class AgentController : Agent
{
    // Pellet Variables
    [SerializeField] private Transform target;
    public int pelletCount;
    public GameObject food;
    [SerializeField] private List<GameObject> spawnedPelletsList = new List<GameObject>();

    // Agent Variables
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;

    // Environment Variables
    Material environmentMaterial;
    public GameObject environment;

    // Time keeping variables
    [SerializeField] private int timeForEpisode;
    private float timeLeft;

    // Enemy Agent
    public HunterController classObject;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        environmentMaterial = environment.GetComponent<Renderer>().material;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-4.5f, 3.5f), 0.25f, Random.Range(-1f, -9.5f)); // Starting position of the agent

        // target.localPosition = new Vector3(Random.Range(-5f, 4f), 0.25f, Random.Range(-1f, -10f)); // Starting position of the target
        CreatePellet(); // Create pellets at the start of the episode

        // Timer to determine if agent has run out of time
        EpisodeTimeNew(); // Reset the timer for the episode
    }

    private void Update()
    {
        CheckRemainingTime(); // Check if the time for the episode has run out
    }

    private void CreatePellet()
    {
        distanceList.Clear();
        badDistanceList.Clear();

        // Clear the previous pellets
        if (spawnedPelletsList.Count != 0)
        {
            RemovePellet(spawnedPelletsList);
        }

        for (int i = 0; i < pelletCount; i++)
        {
            int counter = 0;
            bool distanceBad;
            bool alreadyDecremented = false;

            // Create a new pellet at a random position
            GameObject newPellet = Instantiate(food);
            // Make pellet child of the environment to prevent it from falling through the ground
            newPellet.transform.SetParent(transform.parent);
            // Give it a random position
            Vector3 pelletLocation = new Vector3(Random.Range(-4.5f, 3.5f), 0.25f, Random.Range(-1f, -9.5f));

            if (spawnedPelletsList.Count != 0)
            {
                for (int k = 0; k < spawnedPelletsList.Count; k++)
                {
                    if (counter < 10)
                    {
                        distanceBad = CheckOverlap(pelletLocation, spawnedPelletsList[k].transform.localPosition, 5f);
                        if (distanceBad)
                        {
                            pelletLocation = new Vector3(Random.Range(-4.5f, 3.5f), 0.25f, Random.Range(-1f, -9.5f));
                            k--;
                            alreadyDecremented = true; // Prevent decrementing k multiple times for the same pellet
                            Debug.Log("Too close to other pellet");
                        }

                        distanceBad = CheckOverlap(pelletLocation, transform.localPosition, 5f);
                        if (distanceBad)
                        {
                            Debug.Log("Too close to agent");
                            pelletLocation = new Vector3(Random.Range(-4.5f, 3.5f), 0.25f, Random.Range(-1f, -9.5f));
                            if (!alreadyDecremented)
                            {
                                k--;
                            }
                        }

                        counter++;
                    }
                    else
                    {
                        k = spawnedPelletsList.Count; // Exit the loop if we have tried 10 times
                    }
                }
            }

            // Spawn the pellet at the random position
            newPellet.transform.localPosition = pelletLocation;
            // Add the pellet to the list
            spawnedPelletsList.Add(newPellet);
        }
    }

    public List<float> distanceList = new List<float>();
    public List<float> badDistanceList = new List<float>();

    public bool CheckOverlap(Vector3 objectWeWantToAvoidOverlapping, Vector3 alreadyExistingObject, float minDistanceWanted)
    {
        float distanceBetweenObjects = Vector3.Distance(objectWeWantToAvoidOverlapping, alreadyExistingObject);
        if (distanceBetweenObjects < minDistanceWanted)
        {
            if (!badDistanceList.Contains(distanceBetweenObjects))
            {
                badDistanceList.Add(distanceBetweenObjects);
            }
            return true; // Overlapping
        }
        else
        {
            if (!distanceList.Contains(distanceBetweenObjects))
            {
                distanceList.Add(distanceBetweenObjects);
            }
            return false; // Not overlapping
        }
    }

    private void RemovePellet(List<GameObject> toBeDeletedGameObjectList)
    {
        foreach (GameObject i in toBeDeletedGameObjectList)
        {
            Destroy(i.gameObject);
        }
        toBeDeletedGameObjectList.Clear();
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
        if (other.CompareTag("Pellet"))
        {
            // AddReward(5f);
            // EndEpisode();

            // Remove pellet from the list
            spawnedPelletsList.Remove(other.gameObject);
            // Destroy the pellet
            Destroy(other.gameObject);
            AddReward(10f);
            if (spawnedPelletsList.Count == 0)
            {
                environmentMaterial.color = Color.green; // Change environment color to green when all pellets are collected
                RemovePellet(spawnedPelletsList);
                AddReward(5f);
                classObject.AddReward(-5f); // Penalize the hunter for not catching the prey
                classObject.EndEpisode(); // End the episode for the hunter
                EndEpisode();
            }
        }

        if (other.CompareTag("Wall"))
        {
            environmentMaterial.color = Color.red; // Change environment color to red on collision with wall
            RemovePellet(spawnedPelletsList);
            AddReward(-15f);
            classObject.EndEpisode(); // End the episode for the hunter
            EndEpisode();
        }
    }

    private void EpisodeTimeNew()
    {
        timeLeft = Time.time + timeForEpisode;
    }

    private void CheckRemainingTime()
    {
        if (Time.time > timeLeft)
        {
            environmentMaterial.color = Color.blue; // Change environment color to blue when time runs out
            AddReward(-15f);
            classObject.AddReward(-15f); // Penalize the hunter for not catching the prey
            RemovePellet(spawnedPelletsList);
            classObject.EndEpisode();
            EndEpisode();
        }
    }
}