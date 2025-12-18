using UnityEngine;

[RequireComponent(typeof(Light))]
public class PlayerFlashlightHorror : MonoBehaviour
{
    [Header("Toggle Settings")]
    public KeyCode toggleKey = KeyCode.F;
    private Light flashlight;
    private bool isOn = false;

    [Header("Base Intensity (URP Lumens)")]
    public float baseIntensity = 2500f;

    [Header("Flicker Settings (percent)")]
    [Range(0f, 0.5f)]
    public float flickerPercent = 0.1f; // 10%

    public float flickerSpeed = 20f;

    [Header("Sway Settings")]
    public float swayAmount = 0.4f;
    public float swaySpeed = 2f;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip soundOn;
    public AudioClip soundOff;

    private Vector3 initialRotation;

    void Start()
    {
        flashlight = GetComponent<Light>();
        flashlight.intensity = baseIntensity;
        flashlight.enabled = isOn;
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

            if (audioSource != null)
            {
                if (isOn && soundOn != null)
                    audioSource.PlayOneShot(soundOn);
                else if (!isOn && soundOff != null)
                    audioSource.PlayOneShot(soundOff);
            }
        }
    }

    private void HandleFlicker()
    {
        float flicker = 1f + Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * flickerPercent;
        flashlight.intensity = baseIntensity * flicker;
    }

    private void HandleSway()
    {
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayY = Mathf.Cos(Time.time * swaySpeed * 1.3f) * swayAmount;

        transform.localEulerAngles = initialRotation + new Vector3(swayX, swayY, 0);
    }
}
