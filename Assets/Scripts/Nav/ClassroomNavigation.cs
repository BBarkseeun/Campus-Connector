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
        CheckPositions(); // 위치가 NavMesh 위에 있는지 확인

        string classroomName = classroomInput.text;
        GameObject classroom = GameObject.Find(classroomName);

        if (classroom)
        {
            // 도착 지점이 NavMesh 위에 있는지 확인
            NavMeshHit hit;
            Vector3 destinationOnNavMesh;

            if (NavMesh.SamplePosition(classroom.transform.position, out hit, 1000f, NavMesh.AllAreas))
            {
                Debug.Log("Destination position is on the NavMesh!");
                destinationOnNavMesh = hit.position;  // NavMesh 위의 도착 지점
            }
            else
            {
                Debug.LogError("Destination position is not on the NavMesh!");
                return;  // NavMesh 위에 없으면 경로 계산을 중단
            }

            path = new NavMeshPath();

            // NavMesh.CalculatePath의 반환 값으로 경로 계산 성공 여부 확인
            if (NavMesh.CalculatePath(transform.position, destinationOnNavMesh, NavMesh.AllAreas, path))
            {
                DrawPath(path);
                Debug.Log("강의실위치: " + classroomName);
                Debug.Log("시작위치: " + transform.position);

                // 여기서 NavMeshAgent를 실제로 움직입니다.
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

        if (path.corners.Length < 2) // 경로가 2개 미만의 포인트로 구성된 경우는 그리지 않습니다.
            return;

        lineRenderer.positionCount = path.corners.Length; // 선의 점의 수 설정

        float heightOffset = 10f; // 이 값을 조정하여 높이를 변경할 수 있습니다.

        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 elevatedPosition = path.corners[i] + new Vector3(0, heightOffset, 0);
            lineRenderer.SetPosition(i, elevatedPosition); // 선의 각 점의 위치 설정
        }
    }
}