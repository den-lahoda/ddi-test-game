using UnityEngine;

[RequireComponent(typeof(Light))]
public class PlayerFlashlightHorror : MonoBehaviour
{
    [Header("Toggle Settings")]
    public KeyCode toggleKey = KeyCode.F;  // клавиша включения/выключения
    private Light flashlight;

    [Header("Drift / Flicker Settings")]
    public float flickerIntensityMin = 1.8f; // минимальная яркость при дрожании
    public float flickerIntensityMax = 2.2f; // максимальная яркость
    public float flickerSpeed = 0.1f;        // скорость дрожания света
    public float swayAmount = 1f;            // угол дрожания в градусах
    public float swaySpeed = 2f;             // скорость дрожания камеры

    private bool isOn = false;
    private Vector3 initialRotation;

    void Start()
    {
        flashlight = GetComponent<Light>();
        flashlight.enabled = isOn;            // фонарик изначально выключен
        initialRotation = transform.localEulerAngles;
    }

    void Update()
    {
        HandleToggle();
        if (isOn)
        {
            HandleFlicker();
            HandleSway();
        }
    }

    private void HandleToggle()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
        }
    }

    private void HandleFlicker()
    {
        // Меняем интенсивность света случайным образом для эффекта дрожания
        flashlight.intensity = Random.Range(flickerIntensityMin, flickerIntensityMax);
    }

    private void HandleSway()
    {
        // Лёгкое дрожание фонарика для хоррора
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayY = Mathf.Cos(Time.time * swaySpeed * 1.3f) * swayAmount;
        transform.localEulerAngles = initialRotation + new Vector3(swayX, swayY, 0);
    }
}
