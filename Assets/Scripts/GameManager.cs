using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class TaskUI
    {
        public TextMeshProUGUI taskText;
        public WheelSocket socket;
        public bool isCompleted = false;

        public CarWash carWash;
        public bool isWashTask = false;
    }

    public List<TaskUI> tasks = new List<TaskUI>();
    public List<WheelColor.ColorType> availableColors;


    void Start()
    {
        GenerateRandomTasks();
        UpdateAllTaskTexts();
    }

    void GenerateRandomTasks()
    {
        List<WheelColor.ColorType> colorsToAssign = new List<WheelColor.ColorType>(availableColors);

        System.Random rnd = new System.Random();
        colorsToAssign = colorsToAssign.OrderBy(x => rnd.Next()).ToList();

        for (int i = 0; i < tasks.Count; i++)
        {
            if (i < colorsToAssign.Count)
            {
                tasks[i].socket.requiredColor = colorsToAssign[i];
                tasks[i].isCompleted = false;
            }
        }
    }

    void UpdateAllTaskTexts()
    {
        foreach (var task in tasks)
        {
            UpdateTaskText(task);
        }
    }

    public void UpdateTaskUI(WheelSocket socket)
    {
        UnityEngine.Debug.Log($"UpdateTaskUI вызван для сокета {socket.socketPosition}");

        TaskUI task = tasks.Find(t => t.socket == socket);
        if (task != null)
        {
            UpdateTaskText(task);
        }
        else
        {
            UnityEngine.Debug.LogError("Не найдена задача для этого сокета!");
        }
    }

    void UpdateTaskText(TaskUI task)
    {
        if (task.taskText == null || task.socket == null) return;

        string positionText = GetPositionText(task.socket.socketPosition);
        string colorText = GetColorText(task.socket.requiredColor);

        // Статус гайки
        string nutStatus = task.socket.nutTightened ? "гайка закручена" : "гайка откручена";

        // Статус колеса
        string wheelStatus = task.socket.isCorrectWheelPlaced ? "колесо установлено" : "колесо не установдено";

        task.taskText.text = $"Установить {colorText} колесо {positionText}\n{nutStatus} | {wheelStatus}";

        if (task.isCompleted)
        {
            task.taskText.color = Color.green;
            task.taskText.fontStyle = FontStyles.Strikethrough;
        }
        else
        {
            task.taskText.color = Color.white;
            task.taskText.fontStyle = FontStyles.Normal;
        }
    }

    public void CheckTaskCompletion(WheelSocket socket)
    {
        TaskUI task = tasks.Find(t => t.socket == socket);

        if (task != null)
        {
            task.isCompleted = socket.IsTaskComplete();
            UnityEngine.Debug.Log($"CheckTaskCompletion для {socket.socketPosition}: isCompleted={task.isCompleted}");

            UpdateTaskText(task);
            CheckAllTasksCompleted();
        }
        else
        {
            UnityEngine.Debug.LogError($"Не найден task для сокета {socket.name}");
        }
    }



    void CheckAllTasksCompleted()
    {
        // Проверяем все задачи с колёсами
        bool allWheelTasksCompleted = true;
        bool washCompleted = true;

        foreach (var task in tasks)
        {
            if (task.isWashTask)
            {
                if (task.carWash != null)
                {
                    washCompleted = task.carWash.currentProgress >= 1f;
                }
            }
            else
            {
                if (!task.isCompleted)
                {
                    allWheelTasksCompleted = false;
                }
            }
        }

        if (allWheelTasksCompleted && washCompleted)
        {
            UnityEngine.Debug.Log("ПОБЕДА! Все задачи выполнены!");
        }
    }

    string GetPositionText(WheelSocket.Position pos)
    {
        switch (pos)
        {
            case WheelSocket.Position.FrontLeft: return "спереди слева";
            case WheelSocket.Position.FrontRight: return "спереди справа";
            case WheelSocket.Position.RearLeft: return "сзади слева";
            case WheelSocket.Position.RearRight: return "сзади справа";
            default: return "";
        }
    }

    string GetColorText(WheelColor.ColorType color)
    {
        switch (color)
        {
            case WheelColor.ColorType.Red: return "КРАСНОЕ";
            case WheelColor.ColorType.Blue: return "СИНЕЕ";
            case WheelColor.ColorType.Green: return "ЗЕЛЕНОЕ";
            case WheelColor.ColorType.Yellow: return "ЖЕЛТОЕ";
            default: return "";
        }
    }
}