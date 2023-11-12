using UnityEngine;

public class ResizeObject : MonoBehaviour
{
    void Start()
    {
        ResizeBasedOnScreenSize();
    }

    void ResizeBasedOnScreenSize()
    {
        // ȭ���� �ػ󵵸� ����ϴ�.
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // ������ �Ǵ� �ػ󵵸� �����մϴ�. �� ���� ���ϴ� Ÿ�� �ػ󵵿� ���� �޶����ϴ�.
        float referenceWidth = 1920.0f;
        float referenceHeight = 1080.0f;

        // ���� ȭ���� ������ ���� ũ�⸦ �����մϴ�.
        float widthRatio = screenWidth / referenceWidth;
        float heightRatio = screenHeight / referenceHeight;

        // �� ���� ������ �������� ��� ������Ʈ�� �������� �����մϴ�.
        float scaleRatio = Mathf.Min(widthRatio, heightRatio);

        // ������Ʈ�� �������� �����մϴ�.
        transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
    }
}
