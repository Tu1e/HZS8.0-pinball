using UnityEngine;

public class FlipperController : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode inputKey = KeyCode.LeftArrow;
    public bool useKeyboard = true; // Da li koristiš tastaturu

    [Header("Flipper Settings")]
    public float flipperSpeed = 4000f; // Brzina trzaja
    public float flipperStrength = 10000f; // Snaga motora (Max Motor Torque)
    
    [Header("Audio Settings")]
    public AudioClip hitSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;
    
    private HingeJoint2D hinge;
    private JointMotor2D motor;
    private bool wasActiveLastFrame = false; // Prati da li je bio aktivan prethodni frame
    private bool isButtonPressed = false; // Za UI dugme

    void Start()
    {
        hinge = GetComponent<HingeJoint2D>();
        // Inicijalizacija motora
        motor = hinge.motor;
        motor.maxMotorTorque = flipperStrength;
        hinge.useMotor = true;
    }

    void Update()
    {
        bool isActive = false;

        // Proveri tastaturu (ako je omogućeno)
        if (useKeyboard && Input.GetKey(inputKey))
        {
            isActive = true;
        }

        // Proveri UI dugme
        if (isButtonPressed)
        {
            isActive = true;
        }

        SetFlipperActive(isActive);
    }

    /// <summary>
    /// Poziva se iz UI dugmeta (OnPointerDown)
    /// </summary>
    public void OnButtonDown()
    {
        isButtonPressed = true;
    }

    /// <summary>
    /// Poziva se iz UI dugmeta (OnPointerUp)
    /// </summary>
    public void OnButtonUp()
    {
        isButtonPressed = false;
    }

    public void SetFlipperActive(bool isActive)
    {
        // Pusti zvuk SAMO kada prelazimo iz neaktivnog u aktivno stanje
        if (isActive && !wasActiveLastFrame)
        {
            AudioHelper.Play2DSound(hitSound, soundVolume);
        }
        
        wasActiveLastFrame = isActive;

        // Ako je aktivno, brzina je pozitivna, inace negativna
        motor.motorSpeed = isActive ? flipperSpeed : -flipperSpeed;
        hinge.motor = motor;
    }
}