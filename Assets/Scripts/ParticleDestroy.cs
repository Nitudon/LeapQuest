using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ParticleSystem particle = GetComponent<ParticleSystem>();
        Destroy(gameObject,particle.duration);
	}

}
