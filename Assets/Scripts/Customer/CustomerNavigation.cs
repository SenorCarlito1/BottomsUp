using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Runtime.CompilerServices;

public class CustomerManager : MonoBehaviour
{
    public static List<CustomerNavigation> activeCustomers = new List<CustomerNavigation>();
}

public class CustomerNavigation : MonoBehaviour
{
    [SerializeField] private string enterOrderedPositionsTag = "EnterPosition";
    [SerializeField] private string randomPositionTag = "RandomPosition";
    [SerializeField] private string randomPositionExitTag = "RandomPositionExit";

    private NavMeshAgent customer;
    private int enterIndex = 0;
    private int exitIndex = 0;
    private int currentRandomTargetIndex = -1;
    private bool isEntering = true;
    private bool isExiting = false;
    private List<Transform> enterOrderedPositions = new List<Transform>();
    private List<Transform> randomPositions = new List<Transform>();
    private List<Transform> exitOrderedPositions = new List<Transform>();

    private static List<Transform> occupiedPositions = new List<Transform>(); // Track occupied positions

    private void Awake()
    {
        customer = GetComponent<NavMeshAgent>();
        FindEnterPositions();
        FindRandomPositions();
    }

    private void Start()
    {
        MoveToTarget();
    }

    private void FindEnterPositions()
    {
        GameObject[] enterPositionObjects = GameObject.FindGameObjectsWithTag(enterOrderedPositionsTag);
        enterOrderedPositions.Clear();
        for (int i = enterPositionObjects.Length - 1; i >= 0; i--)
        {
            enterOrderedPositions.Add(enterPositionObjects[i].transform);
        }
        Debug.Log("Enter positions found in reverse order: " + enterOrderedPositions.Count);
    }

    private void FindRandomPositions()
    {
        GameObject[] randomPositionObjects = GameObject.FindGameObjectsWithTag(randomPositionTag);
        GameObject[] exitPositionObjects = GameObject.FindGameObjectsWithTag(randomPositionExitTag);

        randomPositions.Clear();
        foreach (GameObject obj in randomPositionObjects)
        {
            randomPositions.Add(obj.transform);
        }
        foreach (GameObject obj in exitPositionObjects)
        {
            randomPositions.Add(obj.transform);
        }

        Debug.Log("Random positions added: " + randomPositions.Count);
    }

    private void MoveToTarget()
    {
        if (isEntering)
        {
            EnterTarget();
        }
        else if (!isExiting)
        {
            RandomTarget();
        }
        else
        {
            ExitTarget();
        }
    }

    private void EnterTarget()
    {
        if (enterIndex < enterOrderedPositions.Count)
        {
            customer.destination = enterOrderedPositions[enterIndex].position;
            Invoke("CheckEnterTarget", 1f);
        }
        else
        {
            isEntering = false;
            MoveToTarget();
        }
    }

    private void CheckEnterTarget()
    {
        if (customer.remainingDistance < 0.5f)
        {
            enterIndex++;
            EnterTarget();
        }
        else
        {
            Invoke("CheckEnterTarget", 1f);
        }
    }

    private void RandomTarget()
    {
        if (randomPositions.Count > 0)
        {
            do
            {
                // Choose a random target
                currentRandomTargetIndex = Random.Range(0, randomPositions.Count);
                Transform newTarget = randomPositions[currentRandomTargetIndex];

                // Check if the target is already occupied
                if (!occupiedPositions.Contains(newTarget))
                {
                    occupiedPositions.Add(newTarget); // Mark the position as occupied
                    customer.destination = newTarget.position; // Set the destination to the new target
                    break; // Exit the loop once a valid target is found
                }
            }
            while (true); // Retry until a valid target is found

            // Wait for a random delay before checking if the agent reached the target
            Invoke("CheckRandomTarget", 1f);
        }
    }

    private void CheckRandomTarget()
    {
        if (customer.remainingDistance < 0.5f)
        {
            // Once the customer reaches the target, free up the position and check the next target
            Transform currentTarget = randomPositions[currentRandomTargetIndex];
            occupiedPositions.Remove(currentTarget); // Free the position

            // If this was the last target, start the exit sequence
            if (currentRandomTargetIndex == randomPositions.Count - 1)
            {
                Invoke("StartExitSequence", 0f);
            }
            else
            {
                // Move to a new random target after a short delay
                Invoke("RandomTarget", Random.Range(2f, 5f));
            }
        }
        else
        {
            // Continue checking until the customer reaches the target
            Invoke("CheckRandomTarget", 1f);
        }
    }

    private void StartExitSequence()
    {
        Debug.Log("StartExitSequence");
        isExiting = true;
        MoveToTarget();
    }

    private void ExitTarget()
    {
        Debug.Log("ExitTarget");
        if (exitIndex < exitOrderedPositions.Count)
        {
            customer.destination = exitOrderedPositions[exitIndex].position;
            Invoke("CheckExitTarget", 1f);
        }
        else
        {
            // customer has exited the building, despawn them
            Destroy(gameObject);
        }
    }

    private void CheckExitTarget()
    {
        Debug.Log("CheckExitTarget");
        if (customer.remainingDistance < 0.5f)
        {
            exitIndex++;
            if (exitIndex < exitOrderedPositions.Count)
            {
                ExitTarget();
            }
            else
            {
                // customer has exited the building, despawn them
                Destroy(gameObject);
            }
        }
        else
        {
            Invoke("CheckExitTarget", 1f);
        }
    }
}
