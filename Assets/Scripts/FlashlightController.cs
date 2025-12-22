using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    // ================= LIGHT REFERENCES =================

    [Header("Lights")]
    [Tooltip("Мягкий ближний свет для освещения стен рядом")]
    public Light nearLight;

    [Tooltip("Основной дальний луч фонарика")]
    public Light farLight;

    // ================= INPUT =================

    [Header("Input")]
    [Tooltip("Кнопка включения / выключения фонарика")]
    public KeyCode toggleKey = KeyCode.F;

    // ================= DISTANCE LOGIC =================

    [Header("Distance Logic")]
    [Tooltip("Дистанция, при которой считаем объект близким")]
    public float enterNearDistance = 3.8f;

    [Tooltip("Дистанция, после которой считаем объект снова далёким")]
    public float exitNearDistance = 5.5f;

    // ================= LIGHT BEHAVIOUR =================

    [Header("Light Behaviour")]
    [Tooltip("Скорость плавного изменения яркости")]
    public float fadeSpeed = 10f;

    // ================= INTENSITIES =================

    [Header("Target Intensities")]
    [Tooltip("Интенсивность ближнего света")]
    public float nearIntensity = 4f;

    [Tooltip("Интенсивность дальнего света")]
    public float farIntensity = 45f;

    // ================= AUDIO =================

    [Header("Audio")]
    [Tooltip("Звук включения фонарика")]
    public AudioClip flashlightOnSound;

    [Tooltip("Звук выключения фонарика")]
    public AudioClip flashlightOffSound;

    // ================= DEBUG =================

    [Header("Debug")]
    [Tooltip("Включён ли сейчас фонарик")]
    [SerializeField] private bool flashlightEnabled = true;

    [Tooltip("Находимся ли сейчас в ближнем режиме")]
    [SerializeField] private bool isNear;

    Camera cam;
    AudioSource audioSource;

    // ================= UNITY =================

    void Start()
    {
        cam = GetComponentInParent<Camera>();

        // Ищем AudioSource (на Flashlight или у детей)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = GetComponentInChildren<AudioSource>();

        // Начальное состояние
        nearLight.intensity = nearIntensity;
        farLight.intensity  = farIntensity;
    }

    void Update()
    {
        HandleToggleInput();

        if (!flashlightEnabled)
        {
            isNear = false;
            ForceLightsOff();
            return;
        }

        HandleDistanceLogic();
        UpdateLights();
    }

    // ================= INPUT =================

    void HandleToggleInput()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            flashlightEnabled = !flashlightEnabled;
            PlayToggleSound();
        }
    }

    // ================= LOGIC =================

    void HandleDistanceLogic()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward,
            out RaycastHit hit, exitNearDistance))
        {
            if (!isNear && hit.distance <= enterNearDistance)
                isNear = true;
        }
        else
        {
            isNear = false;
        }
    }

    void UpdateLights()
    {
        // Ближний свет всегда мягко работает
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

    void ForceLightsOff()
    {
        nearLight.intensity = 0f;
        farLight.intensity = 0f;
    }

    // ================= AUDIO =================

    void PlayToggleSound()
    {
        if (audioSource == null) return;

        AudioClip clip = flashlightEnabled
            ? flashlightOnSound
            : flashlightOffSound;

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
