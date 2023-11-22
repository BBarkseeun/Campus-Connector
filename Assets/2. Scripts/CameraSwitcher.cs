using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineBrain cinemachineBrain;
    public CinemachineVirtualCameraBase[] virtualCameras;
    private int currentCameraIndex = 0;

    private void Update()
    {
        // TAB Ű�� ������ �� Live Camera ����
        if (Input.GetKeyDown(KeyCode.Tab) && cinemachineBrain != null && virtualCameras.Length > 0)
        {
            currentCameraIndex = (currentCameraIndex + 1) % virtualCameras.Length;
            cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.SetActive(false); // ���� ī�޶� ��Ȱ��ȭ
            virtualCameras[currentCameraIndex].VirtualCameraGameObject.SetActive(true); // �� ī�޶� Ȱ��ȭ
        }
    }
}