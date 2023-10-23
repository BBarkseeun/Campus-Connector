using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class ClassroomNavigation : MonoBehaviour
{
    public TMP_InputField classroomInput; // ���ǽ� �̸� �Է� UI
    private LineRenderer lineRenderer;
    private NavMeshPath path;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found!");
            return;
        }

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            Debug.Log("LineRenderer component added dynamically.");
            
        }
        LineRenderer line = GetComponent<LineRenderer>();
        if (line != null)
        {
            line.startColor = Color.yellow;
            line.endColor = Color.cyan;
        }

        if (lineRenderer != null)
        {
            Debug.Log("LineRenderer component is attached to this object!");
        }
        else
        {
            Debug.LogError("LineRenderer component is NOT attached to this object!");
        }

        lineRenderer.startWidth = 7f;
        lineRenderer.endWidth = 7f;
      
        

    }
    public void OnFindPathButtonPressed()
    {
        CheckPositions(); // ��ġ�� NavMesh ���� �ִ��� Ȯ��

        string classroomName = classroomInput.text;
        GameObject classroom = GameObject.Find(classroomName);

        if (classroom)
        {
            // ���� ������ NavMesh ���� �ִ��� Ȯ��
            NavMeshHit hit;
            Vector3 destinationOnNavMesh;

            if (NavMesh.SamplePosition(classroom.transform.position, out hit, 1000f, NavMesh.AllAreas))
            {
                Debug.Log("Destination position is on the NavMesh!");
                destinationOnNavMesh = hit.position;  // NavMesh ���� ���� ����
            }
            else
            {
                Debug.LogError("Destination position is not on the NavMesh!");
                return;  // NavMesh ���� ������ ��� ����� �ߴ�
            }

            path = new NavMeshPath();

            // NavMesh.CalculatePath�� ��ȯ ������ ��� ��� ���� ���� Ȯ��
            if (NavMesh.CalculatePath(transform.position, destinationOnNavMesh, NavMesh.AllAreas, path))
            {
                DrawPath(path);
                Debug.Log("���ǽ���ġ: " + classroomName);
                Debug.Log("������ġ: " + transform.position);

                // ���⼭ NavMeshAgent�� ������ �����Դϴ�.
                agent.SetDestination(destinationOnNavMesh);
            }
            else
            {
                Debug.LogError("Failed to calculate path from " + transform.position + " to " + destinationOnNavMesh + ".");
            }
        }
        else
        {
            Debug.LogError("Classroom not found!");
        }
    }
    public bool IsPositionOnNavMesh(Vector3 position, float maxDistance)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, maxDistance, NavMesh.AllAreas);
    }

    public void CheckPositions()
    {
        if (!IsPositionOnNavMesh(transform.position, 2000f))
        {
            Debug.LogError("Starting position is not on the NavMesh!");
        }

        string classroomName = classroomInput.text;
        GameObject classroom = GameObject.Find(classroomName);
        if (classroom && !IsPositionOnNavMesh(classroom.transform.position, 1f))
        {
            Debug.LogError("Destination position (classroom) is not on the NavMesh!");
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

        float heightOffset = 10f; // �� ���� �����Ͽ� ���̸� ������ �� �ֽ��ϴ�.

        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 elevatedPosition = path.corners[i] + new Vector3(0, heightOffset, 0);
            lineRenderer.SetPosition(i, elevatedPosition); // ���� �� ���� ��ġ ����
        }
    }
}