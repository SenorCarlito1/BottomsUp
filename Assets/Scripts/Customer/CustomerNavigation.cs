using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Runtime.CompilerServices;

public class CustomerNavigation : MonoBehaviour
{
    [SerializeField] private List<Transform> enterOrderedPositions;
    [SerializeField] private List<Transform> randomPositions;
    [SerializeField] private List<Transform> exitOrderedPositions;

    private NavMeshAgent customer;
    private int enterIndex = 0;
    private int exitIndex = 0;
    private bool isEntering = true;
    private bool isExiting = false;
    int randomIndex;

    private void Awake()
    {
        customer = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        MoveToTarget();
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
        randomIndex = Random.Range(0, randomPositions.Count);
        customer.destination = randomPositions[randomIndex].position;
        Invoke("CheckRandomTarget", 1f); // wait for 1 second before checking if the agent has reached the target
    }

    private void CheckRandomTarget()
    {
        if (customer.remainingDistance < 0.5f)
        {
            if (randomIndex == randomPositions.Count - 1) // if the customer reaches the last target in the random positions
            {
                Invoke("StartExitSequence", 0f); // start the exit sequence immediately
            }
            else
            {
                Invoke("RandomTarget", Random.Range(0f, 0f)); // wait for a random time between 2 and 5 seconds before moving to the next position

            }
        }
        else
        {
            Invoke("CheckRandomTarget", 1f); // wait for another second before checking again
        }
    }

    private void StartExitSequence()
    {
        isExiting = true;
        MoveToTarget();
    }
    private void ExitTarget()
    {
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
