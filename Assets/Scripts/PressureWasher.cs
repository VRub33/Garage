using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PressureWasher : MonoBehaviour
{
    public ParticleSystem waterParticles;    // Система частиц для воды
    public Transform nozzleTip;               // Кончик шланга для луча
    public float totalCleanTime = 30f;        // Сколько секунд нужно мыть
    public float maxDistance = 5f;             // Дальность луча
    public LayerMask carLayer;                 // Слой машины

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;
    private float washSpeed;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);

        washSpeed = 1f / totalCleanTime;

        if (waterParticles != null)
        {
            waterParticles.Stop();
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;

        // Включаем частицы, когда взяли в руку
        if (waterParticles != null)
        {
            waterParticles.Play();
        }
    }

    void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;

        // Выключаем частицы, когда отпустили
        if (waterParticles != null)
        {
            waterParticles.Stop();
        }
    }

    void Update()
    {
        if (nozzleTip == null || !isGrabbed) return;

        UnityEngine.Debug.DrawRay(nozzleTip.position, nozzleTip.forward * maxDistance, Color.cyan);

        // Проверяем попадание луча в машину
        RaycastHit hit;
        if (Physics.Raycast(nozzleTip.position, nozzleTip.forward, out hit, maxDistance, carLayer))
        {
            CarWash car = hit.collider.GetComponent<CarWash>();
            if (car != null)
            {
                car.AddWashProgress(washSpeed * Time.deltaTime);
            }
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}