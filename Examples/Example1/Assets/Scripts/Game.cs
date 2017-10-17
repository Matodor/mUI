using mFramework;
using mFramework.UI;
using UnityEngine;

public class Game : MonoBehaviour
{
    public void Awake()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;

#if UNITY_EDITOR
        mCore.IsDebug = true;
#endif

        mUI.Create(new UISettings
        {
            CameraSettings =
            {
                CameraClearFlags = CameraClearFlags.SolidColor
            }
        });
        mUI.UICamera.Camera.backgroundColor = Color.gray;
        mUI.LoadOSFont("Arial");
        mUI.BaseView.ChildView<PageControllerView>();
    }
}
