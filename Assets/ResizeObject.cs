using UnityEngine;

public class ResizeObject : MonoBehaviour
{
    void Start()
    {
        ResizeBasedOnScreenSize();
    }

    void ResizeBasedOnScreenSize()
    {
        // 화면의 해상도를 얻습니다.
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 기준이 되는 해상도를 설정합니다. 이 값은 원하는 타겟 해상도에 따라 달라집니다.
        float referenceWidth = 1920.0f;
        float referenceHeight = 1080.0f;

        // 현재 화면의 비율에 따라 크기를 조정합니다.
        float widthRatio = screenWidth / referenceWidth;
        float heightRatio = screenHeight / referenceHeight;

        // 더 작은 비율을 기준으로 삼아 오브젝트의 스케일을 조정합니다.
        float scaleRatio = Mathf.Min(widthRatio, heightRatio);

        // 오브젝트의 스케일을 조정합니다.
        transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
    }
}
