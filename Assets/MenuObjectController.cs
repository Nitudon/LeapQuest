using UnityEngine;
using System.Collections;

public class MenuObjectController : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        for(int i=1;i<3;++i)
        {
             transform.Rotate(0f,1f,0f);
        }
	}
}
