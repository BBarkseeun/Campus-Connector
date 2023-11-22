using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineBrain cinemachineBrain;
    public CinemachineVirtualCameraBase[] virtualCameras;
    private int currentCameraIndex = 0;

    private void Update()
    {
        // TAB 키를 눌렀을 때 Live Camera 변경
        if (Input.GetKeyDown(KeyCode.Tab) && cinemachineBrain != null && virtualCameras.Length > 0)
        {
            currentCameraIndex = (currentCameraIndex + 1) % virtualCameras.Length;
            cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.SetActive(false); // 현재 카메라 비활성화
            virtualCameras[currentCameraIndex].VirtualCameraGameObject.SetActive(true); // 새 카메라 활성화
        }
    }
}