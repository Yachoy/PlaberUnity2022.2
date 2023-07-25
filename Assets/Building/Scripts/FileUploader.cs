using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Компонент-помошник, для получения файла
public class FileUploader : MonoBehaviour
{
    private void Start()
    {
        // Нам не нужно его удалять на новой сцене, т.к. система Singletone
        DontDestroyOnLoad(gameObject);
    }

    // Этот метод вызывается из JS через SendMessage
    void FileRequestCallback(string path)
    {
        // Отсылаем полученную ссылку обратно в FileUploaderHelper
        FileUploaderHelper.SetResult(path);
    }
}

public static class FileUploaderHelper
{
    static FileUploader fileUploaderObject;
    static Action<string> pathCallback;

    static FileUploaderHelper()
    {
        string methodName = "FileRequestCallback"; // Не будем использовать рефлекцию, чтобы не усложнять, захардкодим :)
        string objectName = typeof(FileUploaderHelper).Name; // А здесь используем

        // Создаем объект-помошник для системы FileUploader
        var wrapperGameObject = new GameObject(objectName, typeof(FileUploader));
        fileUploaderObject = wrapperGameObject.GetComponent<FileUploader>();

        // Инициализируем JS часть системы FileUploader
        InitFileLoader(objectName, methodName);
    }

    /// <summary>
    /// Запрашивает файл у пользователя.
    /// Должен вызываться при клике пользователя!
    /// </summary>
    /// <param name="callback">Будет вызван после выбора файла пользователем, в качестве параметра передается Http путь к файлу</param>
    /// <param name="extensions">Расширения файлов, которые можно выбрать, пример: ".jpg, .jpeg, .png"</param>
    public static void RequestFile(Action<string> callback, string extensions = ".jpg, .jpeg, .png")
    {
        RequestUserFile(extensions);
        pathCallback = callback;
    }

    /// <summary>
    /// Для внутреннего использования
    /// </summary>
    /// <param name="path">Путь к файлу</param>
    public static void SetResult(string path)
    {
        pathCallback.Invoke(path);
        Dispose();
    }

    private static void Dispose()
    {
        ResetFileLoader();
        pathCallback = null;
    }

    // Ниже мы объявляем внешние функции из нашего .jslib файла
    [DllImport("__Internal")]
    private static extern void InitFileLoader(string objectName, string methodName);

    [DllImport("__Internal")]
    private static extern void RequestUserFile(string extensions);

    [DllImport("__Internal")]
    private static extern void ResetFileLoader();
}