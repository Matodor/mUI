using System;
using Assets.Scripts.Views;
using mFramework;
using mFramework.UI;
using UnityEngine;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
        print("START");

        mCore.Init();
	    mUI.Create(new UISettings
	    {
	        CameraSettings =
	        {
	            CameraClearFlags = CameraClearFlags.SolidColor
	        }
	    });
        mUI.UICamera.Camera.backgroundColor = Color.gray;
        SpritesRepository.LoadAll();
        mCore.Log("SpritesRepository count: " + SpritesRepository.Count());
	    var mainMenu = mUI.BaseView.ChildView<MainMenu>();
    }
	
	// Update is called once per frame
    void Update() 
    {
    }
}
