using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetFileScene : MonoBehaviour
{
    public void UpdateAvatar()
    {
        // ����������� ���� � ������������
        FileUploaderHelper.RequestFile((path) =>
        {
            // ���� ���� ������ - ����������
            if (string.IsNullOrWhiteSpace(path))
                return;

            // ��������� �������� ��� �������� ��������
            StartCoroutine(UploadImage(path));
        });
    }

    // �������� ��� �������� ��������
    IEnumerator UploadImage(string path)
    {
        // ��� ����� �������� ��������
        Texture2D texture;

        // using ��� ��������������� ������ Dispose, ������� ������ �� ���� � �����
        using (UnityWebRequest imageWeb = new UnityWebRequest(path, UnityWebRequest.kHttpVerbGET))
        {
            // ������� "�����������" ��� ������� � �������� �������
            imageWeb.downloadHandler = new DownloadHandlerTexture();

            // ���������� ������, ���������� ����������� ����� �������� ����� �����
            yield return imageWeb.SendWebRequest();

            // �������� �������� �� "�����������"
            texture = ((DownloadHandlerTexture)imageWeb.downloadHandler).texture;
        }

        
    }
}
