using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlash : MonoBehaviour {

	public float waitTimeMin;
	public float waitTimeMax;

	public float flashSpeed;
	public float holdTime;

	private Image _img;

	// Use this for initialization
	void Start () {
		_img = GetComponent<Image> ();
		StartCoroutine (FlashScreen ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public IEnumerator FlashScreen() {
		yield return new WaitForSeconds (Random.value * (waitTimeMax - waitTimeMin) + waitTimeMin);
		float i = 0f;
		while (i < 1f) {
			i += flashSpeed;
			_img.color = new Color (1f, 1f, 1f, i);
			yield return new WaitForSeconds (0.01f);
		}
		yield return new WaitForSeconds (holdTime);
		i = 1f;
		while (i > 0f) {
			i -= flashSpeed;
			_img.color = new Color (1f, 1f, 1f, i);
			yield return new WaitForSeconds (0.01f);
		}
		StartCoroutine (FlashScreen ());
	}
}
