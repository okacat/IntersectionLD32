using UnityEngine;
using System.Collections;

public class FrontTrigger : MonoBehaviour {

    private VehicleController vehicleController;

    void Start()
    {
        // Get the reference to the controller of this vehicle
        vehicleController = this.transform.parent.GetComponent<VehicleController>();

    }

    void OnTriggerEnter(Collider collider)
    {
        vehicleController.TriggerEntered(collider, VehicleController.TriggerType.Front);
    }

    void OnTriggerExit(Collider collider)
    {
        vehicleController.TriggerExited(collider, VehicleController.TriggerType.Front);
    }
}
