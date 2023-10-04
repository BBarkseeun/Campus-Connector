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
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 시
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point); // 클릭한 지점을 목적지로 설정
            }
        }

        // 경로가 완전히 계산되었을 때만 DrawPath를 호출
        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            DrawPath(agent.path); // 경로 그리기
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

        float heightOffset = 19f; // 이 값을 조정하여 높이를 변경할 수 있습니다.

        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 elevatedPosition = path.corners[i] + new Vector3(0, heightOffset, 0);
            lineRenderer.SetPosition(i, elevatedPosition); // 선의 각 점의 위치 설정
        }
    }


}