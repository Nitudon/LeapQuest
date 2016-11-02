using UnityEngine;
using System.Collections;

public class CloudController : MonoBehaviour {

    private const float CLOUD_SPEED = 0.0011f;

	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(CLOUD_SPEED,0f,0f);
	}
}
