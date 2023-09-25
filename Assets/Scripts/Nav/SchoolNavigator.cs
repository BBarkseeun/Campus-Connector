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
        // �ٸ� ���ǽ� ������ ���⿡ �߰�...
    };

    public TMP_InputField roomNumberInput;
    public NavMeshAgent agent;
    public LineRenderer lineRenderer;

    void Start()
    {
        // agent�� lineRenderer ������Ʈ�� �ʱ�ȭ
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
        if (path.status != NavMeshPathStatus.PathInvalid)
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
        }
        else
        {
            Debug.LogWarning("Invalid path!");
        }
    }
}
