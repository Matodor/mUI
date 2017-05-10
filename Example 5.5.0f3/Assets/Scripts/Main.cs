using System;
using Assets.Scripts.Views;
using mFramework;
using UnityEngine;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    mCore.Init();

	    UI.Create();
	    var mainMenu = UI.BaseView.ChildView<MainMenu>(1, 2, "3");
	}
	
	// Update is called once per frame
	void Update () {

    }
}
