using System.Diagnostics;
using UnityEngine;

public class SimpleNut : MonoBehaviour
{
    public WheelSocket parentSocket;
    private MeshRenderer nutRenderer;
    private Collider nutCollider;

    [Header("Состояние")]
    public bool isTightened = true;       // true = закручена, false = откручена
    public bool wheelPresent = true;      // Видна ли гайка (есть ли колесо)

    [Header("Визуал")]
    public Material tightenedMaterial;    // Материал закрученной гайки
    public Material loosenedMaterial;     // Материал открученной гайки

    void Start()
    {
        nutRenderer = GetComponent<MeshRenderer>();
        nutCollider = GetComponent<Collider>();

        // Принудительно обновляем визуал при старте
        UpdateVisual();
    }

    public void OnWheelPlaced()
    {
        wheelPresent = true;
        UpdateVisual();
        UnityEngine.Debug.Log($"SimpleNut: колесо поставлено, wheelPresent=true, isTightened={isTightened}");
    }

    public void OnWheelRemoved()
    {
        wheelPresent = false;
        nutRenderer.enabled = false;
        nutCollider.enabled = false;
        UnityEngine.Debug.Log("SimpleNut: колесо снято, гайка скрыта");
    }

    public void ToggleTighten()
    {
        if (!wheelPresent)
        {
            UnityEngine.Debug.Log("SimpleNut: попытка переключить гайку, но колеса нет");
            return;
        }

        // Переключаем состояние
        isTightened = !isTightened;
        UpdateVisual();

        UnityEngine.Debug.Log($"SimpleNut: состояние изменено, isTightened = {isTightened}");

        if (parentSocket != null)
        {
            parentSocket.OnNutStateChanged();
        }
    }

    void UpdateVisual()
    {
        if (!wheelPresent)
        {
            nutRenderer.enabled = false;
            nutCollider.enabled = false;
            return;
        }

        nutRenderer.enabled = true;
        nutCollider.enabled = true;

        // ВАЖНО: принудительно назначаем материал в зависимости от состояния
        if (isTightened)
        {
            nutRenderer.material = tightenedMaterial;
            UnityEngine.Debug.Log($"UpdateVisual: гайка ЗАКРУЧЕНА, назначен материал {tightenedMaterial?.name}");
        }
        else
        {
            nutRenderer.material = loosenedMaterial;
            UnityEngine.Debug.Log($"UpdateVisual: гайка ОТКРУЧЕНА, назначен материал {loosenedMaterial?.name}");
        }
    }

    public void ResetNut()
    {
        isTightened = true;
        wheelPresent = true;
        UpdateVisual();
    }
}