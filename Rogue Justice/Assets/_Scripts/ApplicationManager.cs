using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour {

    [SerializeField]
    private BoardManager boardManager;

	void Start () {
        boardManager.Initialize();
    }
	
	void Update () {
		
	}
}
