using UnityEngine;

public class StopperSensor : MonoBehaviour
{
    public StopperController stopperController;

    private bool isGateActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pinball") && !isGateActivated)
        {
            // Loptica ulazi - gate ostaje otvoren (trigger)
            isGateActivated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Pinball") && isGateActivated)
        {
            if (stopperController != null)
            {
                // Loptica izlazi - sada zatvaramo gate (postaje tvrd)
                stopperController.ActivateGate();
            }
        }
    }

    public void ResetSensor()
    {
        isGateActivated = false;
        if (stopperController != null)
        {
            stopperController.ResetGate();
        }
    }
}