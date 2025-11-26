using UnityEngine;

[RequireComponent(typeof(Light))]
public class PlayerFlashlightHorror : MonoBehaviour
{
    [Header("Toggle Settings")]
    public KeyCode toggleKey = KeyCode.F;
    private Light flashlight;
    private bool isOn = false;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip soundOn;
    public AudioClip soundOff;

    [Header("Drift / Flicker Settings")]
    public float flickerIntensityMin = 1.8f;
    public float flickerIntensityMax = 2.2f;
    public float flickerSpeed = 0.1f;

    public float swayAmount = 1f;
    public float swaySpeed = 2f;

    private Vector3 initialRotation;

    void Start()
    {
        flashlight = GetComponent<Light>();
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

            // --- Воспроизведение звука ---
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
        flashlight.intensity = Random.Range(flickerIntensityMin, flickerIntensityMax);
    }

    private void HandleSway()
    {
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayY = Mathf.Cos(Time.time * swaySpeed * 1.3f) * swayAmount;

        transform.localEulerAngles = initialRotation + new Vector3(swayX, swayY, 0);
    }
}
