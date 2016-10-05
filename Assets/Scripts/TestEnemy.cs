using UnityEngine;
using System.Collections;

public class TestEnemy : MonoBehaviour {

    [SerializeField]
    GameObject Enemy;

	// Use this for initialization
	void Start () {
	InvokeRepeating("Create",0f,5f);
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void Create()
    {
        GameObject enemy;
        Vector3 test;

        test = new Vector3(Random.Range(-30f,30f),Random.Range(0f,30f),Random.Range(10f,30f));

        enemy = Instantiate(Enemy,test,Enemy.transform.rotation) as GameObject;
    }
}
