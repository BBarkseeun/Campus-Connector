using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;

public class ClassroomNavigation : MonoBehaviourPunCallbacks
{
    public TMP_InputField classroomInput; // 사용자 인풋 필드
    public LineRenderer lineRenderer; // 경로를 그리는 라인 렌더러
    private GameObject character; // 생성된 캐릭터 오브젝트
    private NavMeshAgent characterAgent; // 캐릭터에 부착된 NavMeshAgent
    private Transform target; // 길찾기의 목적지

    void Start()
    {
        // 캐릭터가 생성될 때까지 기다립니다.
        StartCoroutine(WaitForCharacterSpawn());
    }

    IEnumerator WaitForCharacterSpawn()
    {
        // 캐릭터가 생성될 때까지 루프를 돕니다.
        while (character == null)
        {
            character = GameObject.Find("Character Root(Clone)");
            yield return null; // 다음 프레임까지 기다립니다.
        }

        // 캐릭터의 NavMeshAgent를 가져옵니다.
        characterAgent = character.GetComponent<NavMeshAgent>();
        if (characterAgent == null)
        {
            Debug.LogError("NavMeshAgent component not found on the character.");
        }
    }

    // 경로를 찾고 그리는 메소드
    public void FindPathAndDraw()
    {
        string classroomName = classroomInput.text;
        target = GameObject.Find(classroomName)?.transform;

        if (target != null && character != null && characterAgent != null && lineRenderer != null)
        {
            // 캐릭터 위치에서 시작하여 경로 계산
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
        // 캐릭터가 목적지에 도착했는지 확인합니다.
        if (character != null && target != null && Vector3.Distance(character.transform.position, target.position) < 1.0f)
        {
            // 경로를 숨깁니다.
            lineRenderer.positionCount = 0;
        }
    }
}
