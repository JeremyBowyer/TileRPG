using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public GameObject PauseUI;
	private BattleMaster bm;

	private bool paused = false;

	void Start() {
		bm = GameObject.FindGameObjectWithTag ("GameMaster").GetComponent<BattleMaster>();
		PauseUI.SetActive (false);
	
	}

	void Update(){
		
		if(Input.GetButtonDown("Pause")){
			paused = !paused;
			bm.paused = paused;
		}

		if (paused) {
			PauseUI.SetActive (true);
			Time.timeScale = 0;
		}

		if (!paused) {
			PauseUI.SetActive (false);
			Time.timeScale = 1;
		}

	}

	public void Resume() { 
		paused = false;
		bm.paused = false;
	}

	public void Restart() {
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public void MainMenu(){
		SceneManager.LoadScene (0);
	}

	public void Quit(){
		Application.Quit ();
	}


}
