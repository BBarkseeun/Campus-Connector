using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Classroom
{
    public string roomNumber;
    public Vector3 location;
}

public class SchoolNavigator : MonoBehaviour
{
    public List<Classroom> classrooms = new List<Classroom> {
        new Classroom { roomNumber = "1223", location = new Vector3(1234, 250, 10) },
        // 다른 강의실 정보를 여기에 추가...
    };

    public TMP_InputField roomNumberInput;
    public NavMeshAgent agent;
    public LineRenderer lineRenderer;

    void Start()
    {
        // agent와 lineRenderer 컴포넌트를 초기화
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void NavigateToClassroom()
    {
        string roomNumber = roomNumberInput.text;
        foreach (Classroom classroom in classrooms)
        {
            if (classroom.roomNumber == roomNumber)
            {
                agent.SetDestination(classroom.location);
                ShowPath(agent.path);
                return;
            }
        }
        Debug.LogWarning("Room number not found!");
    }

    void ShowPath(NavMeshPath path)
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

