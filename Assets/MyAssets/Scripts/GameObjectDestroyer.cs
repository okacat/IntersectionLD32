using UnityEngine;
using System.Collections;

public class GameObjectDestroyer : MonoBehaviour {

    void OnTriggerEnter(Collider collider)
    {
        Destroy(collider.gameObject);
    }
}
