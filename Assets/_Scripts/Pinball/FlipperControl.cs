using UnityEngine;

public class FlipperController : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode inputKey = KeyCode.LeftArrow;

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
        // Proveravamo input
        if (Input.GetKey(inputKey))
        {
            // Ako drzimo dugme, zelimo da idemo ka gornjem limitu (pozitivna brzina)
            SetFlipperActive(true);
        }
        else
        {
            // Ako pustimo, vracamo se dole (negativna brzina)
            SetFlipperActive(false);
        }
    }

    void SetFlipperActive(bool isActive)
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