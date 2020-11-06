using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.UIFramework;

// Clearly this should be automated in an editor script!
// deVoid offers a good approach
// but I do not want to add any abstraction while this file is still relatively small.

public class ScreenList : MonoBehaviour
{
    #region Window Controllers

    [SerializeField] private ContentWindowController guideContentWindowController;
    public static string guideContentWindow = "Guide Content Window";

    [SerializeField] private ContentWindowController visitorContentWindowController;
    public static string visitorContentWindow = "Visitor Content Window";

    [SerializeField] private ProfileNameWindowController profileNameWindowController;
    public static string profileNameWindow = "Profile Name Window";

    [SerializeField] private ProfileEditWindowController profileEditWindowController;
    public static string profileEditWindow = "Profile Edit Window";

    [SerializeField] private GuideSetupWindowController guideSetupWindowController;
    public static string guideSetupWindow = "Guide Setup Window";

    [SerializeField] private LoadingWindowController loadingWindowController;
    public static string loadingWindow = "Loading Window";

    [SerializeField] private NetworkSettingsWindowController networkConfigurationWindowController;
    public static string networkSettingsWindow = "Network Settings Window";

    [SerializeField] private ModeSelectionWindowController modeSelectionWindowController;
    public static string modeSelectionWindow = "Mode Selection Window";

    [SerializeField] private VisitorSetupWindowController visitorSetupWindowController;
    public static string visitorSetupWindow = "Visitor Setup Window";

    [SerializeField] private ExplorerWindowController explorerWindowController;
    public static string explorerWindow = "Explorer Window";

    #endregion

    private UIFrame uiFrame;

    public void InstantiateAndRegisterScreens(UIFrame frame)
    {
        uiFrame = frame;
        InstantiateAndRegisterScreen(guideContentWindowController, guideContentWindow);
        InstantiateAndRegisterScreen(visitorContentWindowController, visitorContentWindow);
        InstantiateAndRegisterScreen(profileNameWindowController, profileNameWindow);
        InstantiateAndRegisterScreen(profileEditWindowController, profileEditWindow);
        InstantiateAndRegisterScreen(guideSetupWindowController, guideSetupWindow);
        InstantiateAndRegisterScreen(loadingWindowController, loadingWindow);
        InstantiateAndRegisterScreen(networkConfigurationWindowController, networkSettingsWindow);
        InstantiateAndRegisterScreen(modeSelectionWindowController, modeSelectionWindow);
        InstantiateAndRegisterScreen(visitorSetupWindowController, visitorSetupWindow);
        InstantiateAndRegisterScreen(explorerWindowController, explorerWindow);
    }

    private void InstantiateAndRegisterScreen<T>(AUIScreenController<T> screenController, string screenName) where T: IScreenProperties
    {
        var screenInstance = Instantiate(screenController.gameObject);
        screenController = screenInstance.GetComponent<AUIScreenController<T>>(); // some juggling: get the screenController component of the newly instantiated copy

        if (screenController != null)
        {
            uiFrame.RegisterScreen(screenName, screenController, screenInstance.transform);
            screenInstance.SetActive(false);
        }
    }
}