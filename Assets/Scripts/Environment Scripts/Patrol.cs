using UnityEngine;
using UnityEngine.AI;

// This class was written by Unity on their Navigation documentation.
public class Patrol : MonoBehaviour
{

    // Does this patroller need to worry about a blend tree ?
    [SerializeField]
    private bool blendTree = true;

    // The multiplier at which the blend tree factor is affected
    [SerializeField]
    private float multiplier = 100;

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }


    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GotoNextPoint();
        }

        if (blendTree)
        {
            float speedFloat = agent.velocity.sqrMagnitude * multiplier;
            animator.SetFloat("BlendFactor", speedFloat);
        }
    }
}
