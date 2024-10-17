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
    [SerializeField] private List<Transform> exitOrderedPositions;

    private NavMeshAgent customer;
    private int enterIndex = 0;
    private int exitIndex = 0;
    private int currentRandomTargetIndex = -1;
    private bool isEntering = true;
    private bool isExiting = false;
    private List<Transform> enterOrderedPositions = new List<Transform>();
    private List<Transform> randomPositions = new List<Transform>();

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

        // Clear the list before adding new positions

        enterOrderedPositions.Clear();

        // Iterate through the found objects in reverse order

        for (int i = enterPositionObjects.Length - 1; i >= 0; i--)

        {

            enterOrderedPositions.Add(enterPositionObjects[i].transform); // Add to the enter positions list

        }

        Debug.Log("Enter positions found in reverse order: " + enterOrderedPositions.Count);

    }
    private void FindRandomPositions()
    {
        GameObject[] randomPositionObjects = GameObject.FindGameObjectsWithTag(randomPositionTag);
        GameObject[] exitPositionObjects = GameObject.FindGameObjectsWithTag(randomPositionExitTag);
        foreach (GameObject obj in randomPositionObjects)
        {
            randomPositions.Add(obj.transform);
        }
        Debug.Log("ranAdded");
        foreach (GameObject obj in exitPositionObjects)
        {
            randomPositions.Add(obj.transform);
        }
        Debug.Log("extAdded");
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
        if (randomPositions.Count > 0) // Ensure there are random positions available
        {
            do
            {
                currentRandomTargetIndex = Random.Range(0, randomPositions.Count); // Update the index to a new random target
                Transform newTarget = randomPositions[currentRandomTargetIndex];

                bool isTargetOccupied = false;
                foreach (CustomerNavigation customer in CustomerManager.activeCustomers)
                {
                    if (customer != this && customer.customer.destination == newTarget.position)
                    {
                        isTargetOccupied = true;
                        break;
                    }
                }
                if (!isTargetOccupied)
                {
                    break; //target available
                }
            }
            while (true);
            customer.destination = randomPositions[currentRandomTargetIndex].position; // Set the new destination
            Invoke("CheckRandomTarget", 1f); // Wait for 1 second before checking if the agent has reached the target
        }
    }

    private void CheckRandomTarget()
    {
        // Check if the customer has reached the current target
        if (customer.remainingDistance < 0.5f)
        {
            // Ensure currentRandomTargetIndex is valid before accessing the list
            if (currentRandomTargetIndex >= 0 && currentRandomTargetIndex < randomPositions.Count)
            {
                Transform currentTarget = randomPositions[currentRandomTargetIndex]; // Get the current target based on the index
                // Check if the current target is the last target in the list
                if (currentRandomTargetIndex == randomPositions.Count - 1)
                {
                    Invoke("StartExitSequence", 0f); // Start the exit sequence immediately
                }
                else
                {
                    // Move to a new random target after a random delay
                    Invoke("RandomTarget", Random.Range(2f, 5f)); // Wait for a random time between 2 and 5 seconds before moving to the next position
                }
            }
            else
            {
                Debug.LogError("Current random target index is out of range: " + currentRandomTargetIndex);
                RandomTarget(); // Attempt to get a new random target
            }
        }
        else
        {
            Invoke("CheckRandomTarget", 1f); // Wait for another second before checking again
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
