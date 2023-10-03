using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class ClassroomNavigation : MonoBehaviour
{
    public TMP_InputField classroomInput; // 강의실 이름 입력 UI
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
            path = new NavMeshPath(); // 여기서 NavMeshPath 객체를 초기화합니다.
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

        if (path.corners.Length < 2) // 경로가 2개 미만의 포인트로 구성된 경우는 그리지 않습니다.
            return;

        lineRenderer.positionCount = path.corners.Length; // 선의 점의 수 설정

        float heightOffset = 19f; // 이 값을 조정하여 높이를 변경할 수 있습니다.

        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 elevatedPosition = path.corners[i] + new Vector3(0, heightOffset, 0);
            lineRenderer.SetPosition(i, elevatedPosition); // 선의 각 점의 위치 설정
        }
    }
}
