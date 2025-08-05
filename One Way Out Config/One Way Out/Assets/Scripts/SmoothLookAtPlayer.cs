using UnityEngine;

public class SmoothLookAwayFromPlayer : MonoBehaviour
{
    [Header("Настройки поворота")]
    [Tooltip("Целевой объект (игрок)")]
    public Transform targetPlayer;
    
    [Tooltip("Скорость плавного поворота")]
    [Range(0.1f, 20f)] public float rotationSpeed = 5f;

    [Tooltip("Поворот по всем осям?")]
    public bool full3DRotation = false;

    [Tooltip("Вращаться задом к игроку?")]
    public bool lookAwayFromPlayer = true;

    void Update()
    {
        if (targetPlayer == null)
        {
            Debug.LogWarning("Target Player не назначен!");
            return;
        }

        // Направление к/от игрока в зависимости от настройки
        Vector3 direction = lookAwayFromPlayer ? 
            (transform.position - targetPlayer.position) : 
            (targetPlayer.position - transform.position);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation;

            if (full3DRotation)
            {
                // Полный 3D поворот
                targetRotation = Quaternion.LookRotation(direction);
            }
            else
            {
                // Только горизонтальный поворот (игнорируем Y)
                direction.y = 0;
                targetRotation = Quaternion.LookRotation(direction);
            }

            // Плавный поворот
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}