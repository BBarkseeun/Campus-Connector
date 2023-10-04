using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PathDrawer : MonoBehaviour
{
    private NavMeshAgent agent;
    public LineRenderer lineRenderer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found!");
            return;
        }
        /*
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            Debug.Log("LineRenderer component added dynamically.");
        }

        if (lineRenderer != null)
        {
            Debug.Log("LineRenderer component is attached to this object!");
        }
        else
        {
            Debug.LogError("LineRenderer component is NOT attached to this object!");
        }

        lineRenderer.startWidth = 1.5f;
        lineRenderer.endWidth = 1.5f;
        */
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ�� ��
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point); // Ŭ���� ������ �������� ����
            }
        }

        // ��ΰ� ������ ���Ǿ��� ���� DrawPath�� ȣ��
        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            DrawPath(agent.path); // ��� �׸���
        }
    }

    void DrawPath(NavMeshPath path)
    {
        if (path == null)
        {
            Debug.LogError("NavMeshPath is null.");
            return;
        }

        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not set.");
            return;
        }

        if (path.corners.Length < 2) // ��ΰ� 2�� �̸��� ����Ʈ�� ������ ���� �׸��� �ʽ��ϴ�.
            return;

        lineRenderer.positionCount = path.corners.Length; // ���� ���� �� ����

        float heightOffset = 19f; // �� ���� �����Ͽ� ���̸� ������ �� �ֽ��ϴ�.

        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 elevatedPosition = path.corners[i] + new Vector3(0, heightOffset, 0);
            lineRenderer.SetPosition(i, elevatedPosition); // ���� �� ���� ��ġ ����
        }
    }


}