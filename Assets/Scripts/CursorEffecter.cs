using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Imageの点滅スクリプト
/// </summary>

public class CursorEffecter : MonoBehaviour {
	[SerializeField] float uplimit;
	[SerializeField] float downlimit;
	[SerializeField] float speed;
	private Color cursorColor;
	private bool colorTrigger = false;
	private Color alpha;
	// Use this for initialization
	void Start () {
		cursorColor = GetComponent<Image> ().color;
		alpha =  new Color (0f, 0f, 0f, speed);
	}
	
	// Update is called once per frame
	void Update () {
	
	if (colorTrigger == false) {
			cursorColor = GetComponent<Image> ().color;
			cursorColor -= alpha;
			GetComponent<Image> ().color = cursorColor;
			if(cursorColor.a < uplimit)
				colorTrigger = true;
		}

		if (colorTrigger == true) {
			cursorColor = GetComponent<Image> ().color;
			cursorColor += alpha;
			GetComponent<Image> ().color = cursorColor;
			if(cursorColor.a >downlimit)
				colorTrigger = false;
		}
	}
	
}
