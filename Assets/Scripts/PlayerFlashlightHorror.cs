using UnityEngine;

[RequireComponent(typeof(Light))]
public class PlayerFlashlightHorror : MonoBehaviour
{
    [Header("Toggle")]

    [Tooltip("Клавиша включения и выключения фонарика")]
    public KeyCode toggleKey = KeyCode.F;


    [Header("Intensity")]

    [Tooltip("Максимальная яркость фонаря")]
    public float maxIntensity = 6f;

    [Tooltip("Минимальная яркость, когда фонарь почти вплотную к стене")]
    public float minIntensity = 0.3f;


    [Header("Distance Dimming")]

    [Tooltip("Дистанция, на которой фонарь светит с полной яркостью")]
    public float fullIntensityDistance = 3.5f;

    [Tooltip("Минимальная дистанция до поверхности для максимального затемнения")]
    public float closeDistance = 0.5f;

    [Tooltip("Слои объектов, которые затемняют свет (например: стены)")]
    public LayerMask dimmingLayers;


    [Header("Flicker")]

    [Tooltip("Сила мерцания фонаря (0 — без мерцания)")]
    [Range(0f, 0.15f)]
    public float flickerAmount = 0.05f;

    [Tooltip("Скорость изменения мерцания")]
    public float flickerSpeed = 10f;


    [Header("Sway (Mouse Based)")]

    [Tooltip("Амплитуда покачивания фонаря от движения мыши")]
    public float swayAmount = 1.2f;

    [Tooltip("Плавность возврата фонаря (чем больше — тем плавнее)")]
    public float swaySmooth = 6f;


    [Header("Sound")]

    [Tooltip("Источник звука для щелчка фонаря")]
    public AudioSource audioSource;

    [Tooltip("Звук включения фонаря")]
    public AudioClip soundOn;

    [Tooltip("Звук выключения фонаря")]
    public AudioClip soundOff;


    private Light flashlight;
    private Camera cam;
    private bool isOn;

    private Vector3 baseRotation;
    private Vector2 currentSway;
    private Vector2 swayVelocity;

    void Awake()
    {
        flashlight = GetComponent<Light>();
        cam = GetComponentInParent<Camera>();

        flashlight.enabled = false;
        flashlight.intensity = maxIntensity;
        baseRotation = transform.localEulerAngles;
    }

    void Update()
    {
        HandleToggle();

        if (!isOn) return;

        ApplyDistanceDimming();
        ApplyFlicker();
        ApplyMouseSway();
    }

    void HandleToggle()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
            flashlight.enabled = isOn;

            if (audioSource)
                audioSource.PlayOneShot(isOn ? soundOn : soundOff);
        }
    }

    void ApplyDistanceDimming()
    {
        float targetIntensity = maxIntensity;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, fullIntensityDistance, dimmingLayers))
        {
            float angle = Vector3.Angle(-hit.normal, cam.transform.forward);

            if (angle < 45f)
            {
                float t = Mathf.InverseLerp(closeDistance, fullIntensityDistance, hit.distance);
                targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, t);
            }
        }

        flashlight.intensity = Mathf.Lerp(
            flashlight.intensity,
            targetIntensity,
            Time.deltaTime * 8f
        );
    }

    void ApplyFlicker()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
        float flicker = Mathf.Lerp(1f - flickerAmount, 1f, noise);
        flashlight.intensity *= flicker;
    }

    void ApplyMouseSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector2 targetSway = new Vector2(
            -mouseY * swayAmount,
            mouseX * swayAmount
        );

        currentSway = Vector2.SmoothDamp(
            currentSway,
            targetSway,
            ref swayVelocity,
            1f / swaySmooth
        );

        transform.localEulerAngles = baseRotation + new Vector3(
            currentSway.x,
            currentSway.y,
            0f
        );
    }
}
