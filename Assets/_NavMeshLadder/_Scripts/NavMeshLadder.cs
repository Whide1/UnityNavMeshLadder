using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[
    RequireComponent(typeof(OffMeshLink)),
    RequireComponent(typeof(BoxCollider))       //Trigger is required to check if player entering
    ]
public class NavMeshLadder : MonoBehaviour
{

    OffMeshLink ofl;

    private NavMeshAgent lastAgent;
    private List<NavMeshAgent> agentsOnLadder = new List<NavMeshAgent>();
    [SerializeField]
    private Transform[] ladderPoints;
    private bool isEnabled = true;
    void Start()
    {
        NavMeshAgent agent = new NavMeshAgent();
        ofl = GetComponent<OffMeshLink>();
    }
#if UNITY_EDITOR
    //Visaulization in Editor
    private void OnDrawGizmosSelected()
    {
        #region VisualizePoints
        if (ofl == null)
        {
            ofl = GetComponent<OffMeshLink>();
        }

        Vector3 cubeSize = new Vector3(0.2f, 0.2f, 0.2f);
        if (ladderPoints == null)
        {
            return;
        }

        Gizmos.color = Color.blue;
        for (int i = 0; i < ladderPoints.Length; i++)
        {
            Gizmos.DrawCube(ladderPoints[i].position, cubeSize);
        }
        Gizmos.color = Color.red;
        for (int i = 1; i < ladderPoints.Length; i++)
        {
            Gizmos.DrawLine(ladderPoints[i - 1].position, ladderPoints[i].position);
        }

        #endregion
    }
#endif


    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        NavMeshAgent agent = other.GetComponentInParent<NavMeshAgent>();
        if (agent)
        {
            if ((agent.isOnOffMeshLink && !lastAgent) ||
                (lastAgent && agent.isOnOffMeshLink &&
                                (lastAgent.transform.position.y > agent.transform.position.y + agent.height ||
                                    lastAgent.transform.position.y < agent.transform.position.y - agent.height))
                )
            {
                if (!agentsOnLadder.Contains(agent))
                {
                    agentsOnLadder.Add(agent);
                    StartCoroutine(ClimbLadder(agent));
                    lastAgent = agent;
                }
            }



        }
    }

    private IEnumerator ClimbLadder(NavMeshAgent agent)
    {

        //Saving the path so after the climbing is finished it can continue its path
        NavMeshPath path = agent.path;
        agent.enabled = false;

        // Determining if the agent wants to climb up or down
        bool toTop = Vector3.Distance(agent.transform.position, ofl.startTransform.position) < Vector3.Distance(agent.transform.position, ofl.endTransform.position);

        #region Look at ladder
        float t = 0.0f;
        float timeToLookAtLadder = 0.3f;
        Vector3 agentForwardAtStart = agent.transform.forward;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / timeToLookAtLadder;
            Vector3 lerped = Vector3.Lerp(agentForwardAtStart, -transform.forward, t);
            agent.transform.forward = lerped;
            yield return null;
        }
        agent.transform.forward = -transform.forward;
        #endregion

        #region Movement on ladder
        if (toTop)
        {
            for (int i = 0; i < ladderPoints.Length; i++)
            {
                while (agent.transform.position != ladderPoints[i].position)
                {
                    //urPos = agent.transform.position;
                    Vector3 newPosition = Vector3.MoveTowards(agent.transform.position, ladderPoints[i].position, agent.speed * Time.deltaTime);
                    agent.transform.position = newPosition;
                    yield return null;
                }
            }
        }
        else
        {
            for (int i = ladderPoints.Length - 1; i > -1; i--)
            {
                while (agent.transform.position != ladderPoints[i].position)
                {
                    //urPos = agent.transform.position;
                    Vector3 newPosition = Vector3.MoveTowards(agent.transform.position, ladderPoints[i].position, agent.speed * Time.deltaTime);
                    agent.transform.position = newPosition;
                    yield return null;
                }

            }
        }
        #endregion

        agentsOnLadder.RemoveAt(agentsOnLadder.IndexOf(agent));
        agent.enabled = true;
        agent.CompleteOffMeshLink();
        agent.path = path;
        if (lastAgent == agent)
        {
            lastAgent = null;
        }
        yield return null;
    }
    private IEnumerator DisableLadder(float timeToDisable)
    {
        isEnabled = false;
        yield return new WaitForSeconds(timeToDisable);
        Debug.Log("Ladder Enabled!");
        isEnabled = true;
    }
}
