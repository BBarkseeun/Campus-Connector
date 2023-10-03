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


    }
    public void OnFindPathButtonPressed()
    {
        string classroomName = classroomInput.text;
        GameObject classroom = GameObject.Find(classroomName);

        if (classroom)
        {
            path = new NavMeshPath(); // ���⼭ NavMeshPath ��ü�� �ʱ�ȭ�մϴ�.
            NavMesh.CalculatePath(transform.position, classroom.transform.position, NavMesh.AllAreas, path);
            DrawPath(path);
            Debug.Log("Found classroom object: " + classroomName);
            Debug.Log("Starting position: " + transform.position);
        }
        else
        {
            Debug.LogError("Classroom not found!");
           
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 100f);
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
