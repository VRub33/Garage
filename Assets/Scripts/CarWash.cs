using UnityEngine;
using TMPro;
using System.Diagnostics;

public class CarWash : MonoBehaviour
{
    [Header("Настройки мойки")]
    public float totalCleanTime = 30f;     // Сколько секунд нужно мыть всю машину
    public float currentProgress = 0f;     // Текущий прогресс (0-1)

    [Header("Визуал")]
    public Material[] dirtLevels;          // Материалы от грязного к чистому
    public TextMeshProUGUI progressText;   // Текст на панели задач

    private Renderer carRenderer;
    private bool isClean = false;

    void Start()
    {
        carRenderer = GetComponent<Renderer>();
        if (carRenderer != null && dirtLevels.Length > 0)
        {
            carRenderer.material = dirtLevels[4]; // Начинаем с самого грязного
        }

        UpdateUI();
    }

    public void AddWashProgress(float amount)
    {
        if (isClean) return;

        currentProgress += amount;
        currentProgress = Mathf.Clamp01(currentProgress);

        // Обновляем материал в зависимости от прогресса
        UpdateMaterial();
        UpdateUI();

        if (currentProgress >= 1f)
        {
            MakeClean();
        }
    }

    void UpdateMaterial()
    {
        if (carRenderer == null || dirtLevels.Length == 0) return;

        // Определяем, какой материал использовать
        int totalLevels = dirtLevels.Length;
        int materialIndex = Mathf.FloorToInt(currentProgress * (totalLevels - 1));

        materialIndex = totalLevels - 1 - materialIndex;

        carRenderer.material = dirtLevels[materialIndex];
    }

    void UpdateUI()
    {
        if (progressText != null)
        {
            int percent = Mathf.RoundToInt(currentProgress * 100);
            progressText.text = $"Мойка: {percent}%";

            if (isClean)
            {
                progressText.text = "Машина полностью чистая!";
                progressText.color = Color.green;
                progressText.fontStyle = FontStyles.Strikethrough;
            }
        }
    }

    void MakeClean()
    {
        isClean = true;
        UnityEngine.Debug.Log("Машина чистая.");
        UpdateUI();
    }

    public void ResetWash()
    {
        currentProgress = 0f;
        isClean = false;
        UpdateMaterial();
        UpdateUI();
    }
}