using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ConsoleToGUI : MonoBehaviour
{
    public GameObject messagePrefab; // ������ ���������
    public Transform messageContainer; // ��������� ��� ���������

    // �������� ������ �� ���� �������
    public Sprite errorSprite;
    public Sprite warningSprite;
    public Sprite notificationSprite;

    public List<string> logMessages;

    private void Awake()
    {
        logMessages = new List<string>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;

        string filePath = Path.Combine(Application.persistentDataPath, "debug.log");
        if (File.Exists(filePath)) File.Delete(filePath);
        File.WriteAllLines(filePath, logMessages);
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // �������� ����� ��������� �� �������
        GameObject newMessage = Instantiate(messagePrefab, messageContainer);
        TextMeshProUGUI uiText = newMessage.GetComponentInChildren<TextMeshProUGUI>(); // ��� Text
        Image backgroundImage = newMessage.GetComponentInChildren<Image>();

        // ���������� ����� ���������
        uiText.text = logString;

        logMessages.Add($"[{DateTime.Now}] [{type.ToString()}] {logString} - {stackTrace}");



        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                backgroundImage.sprite = errorSprite;
                break;
            case LogType.Warning:
                backgroundImage.sprite = warningSprite;
                break;
            default:
                backgroundImage.sprite = notificationSprite;
                break;
        }

        
        StartCoroutine(DisplayMessage(newMessage));
    }

    IEnumerator DisplayMessage(GameObject message)
    {
        CanvasGroup canvasGroup = message.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = message.AddComponent<CanvasGroup>();
        }

        // ����������� ���������
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            canvasGroup.alpha = t;
            yield return null;
        }
        canvasGroup.alpha = 1;

        // �������� ����� �������������
        yield return new WaitForSeconds(2);

        // ����������� ������������
        for (float t = 1; t > 0; t -= Time.deltaTime)
        {
            canvasGroup.alpha = t;
            yield return null;
        }
        canvasGroup.alpha = 0;

        // ������� ���������
        Destroy(message);
    }
}
