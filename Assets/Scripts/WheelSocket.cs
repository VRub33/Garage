using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System.Diagnostics;

public class WheelSocket : MonoBehaviour
{
    public enum Position { FrontLeft, FrontRight, RearLeft, RearRight }
    public Position socketPosition;
    public WheelColor.ColorType requiredColor;

    public SimpleNut nut;

    public UnityEvent onWheelPlaced;
    public UnityEvent onWheelRemoved;

    public bool nutTightened = true;
    public bool isCorrectWheelPlaced = false;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;
    private GameObject currentWheel = null;

    void Start()
    {
        socketInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();

        socketInteractor.selectEntered.AddListener(OnWheelPlaced);
        socketInteractor.selectExited.AddListener(OnWheelRemoved);

        if (nut != null)
        {
            nut.parentSocket = this;
        }
    }

    void OnWheelPlaced(SelectEnterEventArgs args)
    {
        currentWheel = args.interactableObject.transform.gameObject;

        WheelColor wheelColorComp = currentWheel.GetComponent<WheelColor>();
        if (wheelColorComp != null)
        {
            isCorrectWheelPlaced = (wheelColorComp.wheelColor == requiredColor);
            UnityEngine.Debug.Log($"OnWheelPlaced: цвет колеса={wheelColorComp.wheelColor}, required={requiredColor}, isCorrect={isCorrectWheelPlaced}");

            if (isCorrectWheelPlaced)
            {
                UnityEngine.Debug.Log($"✅ Правильно! {socketPosition} получил {requiredColor} колесо");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"На колесе нет компонента WheelColor!");
            isCorrectWheelPlaced = false;
        }

        onWheelPlaced?.Invoke();


        // после установки колеса проверяем задачу
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.CheckTaskCompletion(this);
        }
    }

    void OnWheelRemoved(SelectExitEventArgs args)
    {
        UnityEngine.Debug.Log($"OnWheelRemoved для {socketPosition}, было isCorrectWheelPlaced={isCorrectWheelPlaced}");

        isCorrectWheelPlaced = false;
        currentWheel = null;

        onWheelRemoved?.Invoke();

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.UpdateTaskUI(this);
        }
    }

    public void OnNutStateChanged()
    {
        if(nut != null)
    {
            bool previousState = nutTightened;
            nutTightened = nut.isTightened;

            UnityEngine.Debug.Log($"OnNutStateChanged: гайка изменилась с {previousState} на {nutTightened}");

            // Если состояние реально изменилось
            if (previousState != nutTightened)
            {
                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    gm.UpdateTaskUI(this);
                    gm.CheckTaskCompletion(this);
                }
            }
        }
    }

    public bool IsTaskComplete()
    {
        bool result = nutTightened && isCorrectWheelPlaced;
        UnityEngine.Debug.Log($"IsTaskComplete для {socketPosition}: nutTightened={nutTightened}, isCorrectWheelPlaced={isCorrectWheelPlaced}, result={result}");
        return result;
    }

    public void ResetSocket()
    {
        if (nut != null)
        {
            nut.ResetNut();
            nutTightened = true;
        }
        isCorrectWheelPlaced = false;
    }
}