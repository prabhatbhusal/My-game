using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    // Public variables
    public AudioClip turnOnSound;
    public AudioClip turnOffSound;
    public float verticalRotationSpeed = 100f; // Speed of vertical rotation

    // Private variables
    private Light flashlight;
    private AudioSource audioSource;
    private float verticalRotation = 0f;

    private void Start()
    {
        // Get Light component in the same GameObject
        flashlight = GetComponent<Light>();

        if (flashlight == null)
        {
            Debug.LogWarning("Light component is not attached. Attach a Light component manually.");
        }
        else
        {
            flashlight.enabled = false;
        }

        // Get or add AudioSource component to the same GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        HandleFlashlightToggle();
        HandleFlashlightVerticalMovement();
    }

    private void HandleFlashlightToggle()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashlight != null)
            {
                flashlight.enabled = !flashlight.enabled;

                // Play audio effect based on flashlight state
                if (flashlight.enabled)
                {
                    PlayAudioEffect(turnOnSound);
                }
                else
                {
                    PlayAudioEffect(turnOffSound);
                }
            }
            else
            {
                Debug.LogWarning("Cannot control flashlight as Light component is not attached.");
            }
        }
    }

    private void HandleFlashlightVerticalMovement()
    {
        float mouseY = Input.GetAxis("Mouse Y"); // Get vertical mouse movement
        verticalRotation -= mouseY * verticalRotationSpeed * Time.deltaTime; // Adjust rotation based on speed
        verticalRotation = Mathf.Clamp(verticalRotation, -60f, 60f); // Limit vertical movement angle

        // Apply rotation to the flashlight
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void PlayAudioEffect(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
