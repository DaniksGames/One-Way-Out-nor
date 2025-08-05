using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Tooltip("Ось вращения (X,Y,Z)")]
    public Vector3 rotationAxis = Vector3.up; // По умолчанию вращение вокруг оси Y (вверх)
    
    [Tooltip("Скорость вращения (градусы в секунду)")]
    public float rotationSpeed = 90f;
    
    [Tooltip("Вращать в локальных (true) или глобальных (false) координатах")]
    public bool useLocalSpace = true;

    void Update()
    {
        // Вычисляем угол поворота на этом кадре
        float angle = rotationSpeed * Time.deltaTime;
        
        // Создаем кватернион поворота
        Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis.normalized);
        
        // Применяем вращение
        if(useLocalSpace)
        {
            transform.localRotation *= rotation;
        }
        else
        {
            transform.rotation *= rotation;
        }
    }
}