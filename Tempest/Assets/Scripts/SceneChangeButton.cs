using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour {

	public int newScene;

	private Button _button;

	// Use this for initialization
	void Start () {
		_button = GetComponent<Button> ();
		_button.onClick.AddListener (ChangeScene);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ChangeScene() {
		SceneManager.LoadScene (newScene);
	}

}
 