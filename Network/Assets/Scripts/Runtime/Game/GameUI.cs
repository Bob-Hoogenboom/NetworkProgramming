using System;
using TMPro;
using UnityEngine;

public enum CameraType
{
    MENU = 0,
    WHITE_TEAM = 1,
    BLACK_TEAM = 2,
    LOCAL_GAME = 3
}

public class GameUI : MonoBehaviour
{
    public static GameUI instance { set; get; }

    public Server server;
    public Client client;

    [Header("References")]
    public Action<bool> setLocalGame;

    [SerializeField] private Animator anim;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private GameObject[] cmVCameras;
    [Space]
    private int startMenu = Animator.StringToHash("StartMenu");
    private int onlineMenuHash = Animator.StringToHash("OnlineMenu");
    private int hotsMenuHash = Animator.StringToHash("HostMenu");
    private int inGameHash = Animator.StringToHash("InGame");


    private void Awake()
    {
        instance = this;
        RegisterEvent();
    }

    #region Events
    private void RegisterEvent()
    {
        NetUtility.C_START_GAME += OnStartGameClient;
    }

    private void UnregisterEvents()
    {
        NetUtility.C_START_GAME -= OnStartGameClient;
    }

    private void OnStartGameClient(NetMessage msg)
    {
        anim.SetTrigger(inGameHash);
    }
    #endregion

    #region Cameras
    public void ChangeCamera(CameraType index)
    {
        for (int i = 0; i < cmVCameras.Length; i++)
        {
            cmVCameras[i].SetActive(false);
        }

        cmVCameras[(int)index].SetActive(true);
    }
    #endregion

    #region Game Start Menu
    public void OnLocalPlayBTN()
    {
        anim.SetTrigger(inGameHash);

        setLocalGame?.Invoke(true);

        server.Init(8007);
        client.Init("127.0.0.1", 8007);
    }

    public void OnOnlinePlayBTN()
    {
        anim.SetTrigger(onlineMenuHash);
    }
    #endregion

    #region Online Menu
    public void OnOnlineHostBTN()
    {
        setLocalGame?.Invoke(false);

        server.Init(8007);
        client.Init("127.0.0.1", 8007);

        anim.SetTrigger(hotsMenuHash);
    }

    public void OnOnlineConnectBTN()
    {
        setLocalGame?.Invoke(false);

        client.Init(addressInput.text, 8007);
    }

    public void OnOnlineBackBTN()
    {
        anim.SetTrigger(startMenu);
    }
    #endregion

    #region Host Menu
    public void OnHostBackBTN()
    {
        server.Shutdown();
        client.Shutdown();
        anim.SetTrigger(onlineMenuHash);
    }

    public void OnLeaveFromGameMenu()
    {
        ChangeCamera(CameraType.MENU);
        anim.SetTrigger(startMenu);
    }
    #endregion
}
