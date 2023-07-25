using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetFileScene : MonoBehaviour
{
    public void UpdateAvatar()
    {
        // Запрашиваем файл у пользователя
        FileUploaderHelper.RequestFile((path) =>
        {
            // Если путь пустой - игнорируем
            if (string.IsNullOrWhiteSpace(path))
                return;

            // Запускаем корутину для загрузки картинки
            StartCoroutine(UploadImage(path));
        });
    }

    // Корутина для загрузки картинки
    IEnumerator UploadImage(string path)
    {
        // Тут будет хранится текстура
        Texture2D texture;

        // using для автоматического вызова Dispose, создаем запрос по пути к файлу
        using (UnityWebRequest imageWeb = new UnityWebRequest(path, UnityWebRequest.kHttpVerbGET))
        {
            // Создаем "скачиватель" для текстур и передаем запросу
            imageWeb.downloadHandler = new DownloadHandlerTexture();

            // Отправляем запрос, выполнение продолжится после загрузки всего файла
            yield return imageWeb.SendWebRequest();

            // Получаем текстуру из "скачивателя"
            texture = ((DownloadHandlerTexture)imageWeb.downloadHandler).texture;
        }

        
    }
}
