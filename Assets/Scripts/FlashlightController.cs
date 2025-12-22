using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    // ================= LIGHT REFERENCES =================

    [Header("Lights")]
    [Tooltip("Мягкий ближний свет. Работает всегда, подсвечивает стены рядом")]
    public Light nearLight;

    [Tooltip("Основной дальний луч фонарика. Гаснет вблизи объектов")]
    public Light farLight;

    // ================= DISTANCE LOGIC =================

    [Header("Distance Logic")]
    [Tooltip("Дистанция, при которой считаем объект очень близким (входим в ближний режим)")]
    public float enterNearDistance = 2.5f;

    [Tooltip("Дистанция, после которой считаем объект снова далёким (выходим из ближнего режима)")]
    public float exitNearDistance = 3.5f;

    // ================= LIGHT BEHAVIOUR =================

    [Header("Light Behaviour")]
    [Tooltip("Скорость плавного изменения яркости света (реальная скорость, не коэффициент)")]
    public float fadeSpeed = 12f;

    // ================= INTENSITIES =================

    [Header("Target Intensities")]
    [Tooltip("Целевая интенсивность ближнего света")]
    public float nearIntensity = 4f;

    [Tooltip("Целевая интенсивность дальнего света")]
    public float farIntensity = 45f;

    // ================= DEBUG =================

    [Header("Debug")]
    [Tooltip("Находимся ли сейчас в ближнем режиме")]
    [SerializeField] private bool isNear;

    Camera cam;

    void Start()
    {
        cam = GetComponentInParent<Camera>();

        // Начальные значения (на случай старта рядом со стеной)
        nearLight.intensity = nearIntensity;
        farLight.intensity = farIntensity;
    }

    void Update()
    {
        HandleDistanceLogic();
        UpdateLights();
    }

    // ================= LOGIC =================

    void HandleDistanceLogic()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward,
            out RaycastHit hit, exitNearDistance))
        {
            if (!isNear && hit.distance <= enterNearDistance)
            {
                isNear = true;
            }
        }
        else
        {
            if (isNear)
            {
                isNear = false;
            }
        }
    }

    void UpdateLights()
    {
        // Ближний свет всегда стремится к своей интенсивности
        nearLight.intensity = Mathf.MoveTowards(
            nearLight.intensity,
            nearIntensity,
            fadeSpeed * Time.deltaTime
        );

        // Дальний свет зависит от режима
        float targetFar = isNear ? 0f : farIntensity;

        farLight.intensity = Mathf.MoveTowards(
            farLight.intensity,
            targetFar,
            fadeSpeed * Time.deltaTime
        );
    }
}
