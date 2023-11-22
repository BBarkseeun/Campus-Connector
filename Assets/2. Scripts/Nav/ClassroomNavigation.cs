using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;

public class ClassroomNavigation : MonoBehaviourPunCallbacks
{
    public TMP_InputField classroomInput; // ����� ��ǲ �ʵ�
    public LineRenderer lineRenderer; // ��θ� �׸��� ���� ������
    private GameObject character; // ������ ĳ���� ������Ʈ
    private NavMeshAgent characterAgent; // ĳ���Ϳ� ������ NavMeshAgent
    private Transform target; // ��ã���� ������

    void Start()
    {
        // ĳ���Ͱ� ������ ������ ��ٸ��ϴ�.
        StartCoroutine(WaitForCharacterSpawn());
    }

    IEnumerator WaitForCharacterSpawn()
    {
        // ĳ���Ͱ� ������ ������ ������ ���ϴ�.
        while (character == null)
        {
            character = GameObject.Find("Character Root(Clone)");
            yield return null; // ���� �����ӱ��� ��ٸ��ϴ�.
        }

        // ĳ������ NavMeshAgent�� �����ɴϴ�.
        characterAgent = character.GetComponent<NavMeshAgent>();
        if (characterAgent == null)
        {
            Debug.LogError("NavMeshAgent component not found on the character.");
        }
    }

    // ��θ� ã�� �׸��� �޼ҵ�
    public void FindPathAndDraw()
    {
        string classroomName = classroomInput.text;
        target = GameObject.Find(classroomName)?.transform;

        if (target != null && character != null && characterAgent != null && lineRenderer != null)
        {
            // ĳ���� ��ġ���� �����Ͽ� ��� ���
            NavMeshPath path = new NavMeshPath();
            if (characterAgent.CalculatePath(target.position, path))
            {
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPositions(path.corners);
            }
            else
            {
                Debug.LogError("Path could not be calculated.");
            }
        }
        else
        {
            Debug.LogError("Make sure the classroom, character, character agent, and line renderer are properly set.");
        }
    }

    void Update()
    {
        // ĳ���Ͱ� �������� �����ߴ��� Ȯ���մϴ�.
        if (character != null && target != null && Vector3.Distance(character.transform.position, target.position) < 1.0f)
        {
            // ��θ� ����ϴ�.
            lineRenderer.positionCount = 0;
        }
    }
}
