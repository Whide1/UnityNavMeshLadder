using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentDebugger : MonoBehaviour
{
    NavMeshAgent agent;
    bool MoveAcrossNavMeshesStarted = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
    }

    void Update()
    {
        

        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (agent.enabled && Physics.Raycast(ray, out hit, 1000.0f))
            {
                agent.SetDestination(hit.point);
            }


        }
        //if (agent.isOnOffMeshLink && !MoveAcrossNavMeshesStarted)
        //{
        //    Debug.Log("OffMesh");
        //    StartCoroutine(MoveAcrossNavMeshLink());
        //    MoveAcrossNavMeshesStarted = true;
        //}
    }

    IEnumerator MoveAcrossNavMeshLink()
    {

        //agent.enabled = false;
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        agent.updateRotation = false;

        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float duration = (endPos - startPos).magnitude / agent.velocity.magnitude;
        float t = 0.0f;
        float tStep = 1.0f / duration;
        while (t < 1.0f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            agent.destination = transform.position;
            t += tStep * Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        agent.updateRotation = true;
        agent.CompleteOffMeshLink();
        MoveAcrossNavMeshesStarted = false;
        //agent.enabled = true;

    }
}
