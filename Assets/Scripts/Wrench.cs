using System.Diagnostics;
using UnityEngine;

public class Wrench : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, коснулись ли мы гайки
        SimpleNut nut = other.GetComponent<SimpleNut>();
        if (nut != null)
        {
            // Переключаем состояние гайки
            nut.ToggleTighten();
        }
    }
}