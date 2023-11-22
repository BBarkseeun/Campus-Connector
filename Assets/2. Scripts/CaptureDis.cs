using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CaptureDis : MonoBehaviour
{
    public Camera captureCamera; // ĸó�� ī�޶� ����
    public GameObject panel; // Panel ������Ʈ
    private bool isPanelActive = false; // panel�� ���� Ȱ��ȭ ����
    public Image imageUI; // Image UI ���

    private void Start()
    {
        // �ʱ�ȭ �ڵ� ���� �߰��� �� �ֽ��ϴ�.
    }

    public void OnCaptureButtonClick()
    {
        StartCoroutine(CaptureAndDisplayImage());
        TogglePanel(); // ��ư�� ���� ������ Panel�� Ȱ��ȭ ���¸� ���
    }

    IEnumerator CaptureAndDisplayImage()
    {
        yield return new WaitForEndOfFrame();

        int captureWidth = Screen.width - 1200; // 원하는 너비로 수정
        int captureHeight = Screen.height;

        Texture2D screenCapture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        screenCapture.ReadPixels(new Rect(600, 0, captureWidth, captureHeight), 0, 0); // 600픽셀을 왼쪽으로 이동
        screenCapture.Apply();
        // ����� ���� ��� ���
        string userFolder = GetUserFolder();

        // �̹����� ���Ϸ� ����
        byte[] bytes = screenCapture.EncodeToPNG();
        string filePath = Path.Combine(userFolder, "capturedImage.png");
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Image saved to: " + filePath);

        // Texture2D�� Image�� ǥ��
        Sprite capturedSprite = LoadSprite(filePath);
        imageUI.sprite = capturedSprite;

        // �߰����� ������ ���ϴ´�� ������ �� �ֽ��ϴ�.
    }

    private string GetUserFolder()
    {
        string userFolder = "";

        // �� �ü���� �°� ����� ���� ��� ��������
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            userFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            userFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }
        else if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor)
        {
            userFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }

        return userFolder;
    }

    private void TogglePanel()
    {
        isPanelActive = !isPanelActive;
        panel.SetActive(isPanelActive);
    }

    private Sprite LoadSprite(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // �̹��� �����͸� �ؽ�ó�� �ֱ�

        // Sprite�� ��ȯ
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
}






