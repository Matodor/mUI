using System;
using Assets.Scripts.Views;
using mFramework;
using mFramework.UI;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start ()
    {
        mCore.Init();
        mUI.Create(new UISettings
        {
	        CameraSettings =
	        {
	            CameraClearFlags = CameraClearFlags.SolidColor
	        }
        });
        mUI.UICamera.Camera.backgroundColor = Color.gray;
        mUI.LoadFont("Orbitron");
        mUI.LoadFont("Kazmann_Sans");
        mUI.LoadFont("CTCSplash_Rounded");
        mUI.LoadOSFont("Arial");

        SpritesRepository.LoadAll();
        mCore.Log("SpritesRepository count: " + SpritesRepository.Count());
        var mainMenu = mUI.BaseView.ChildView<MainMenu>();
    }
}
