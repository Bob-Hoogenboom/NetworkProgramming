using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI instance { set; get; }

    [Header("References")]
    [SerializeField] private Animator anim;
    [Space]
    private int startMenu = Animator.StringToHash("StartMenu");
    private int onlineMenuHash = Animator.StringToHash("OnlineMenu");
    private int hotsMenuHash = Animator.StringToHash("HostMenu");
    private int inGameHash = Animator.StringToHash("InGame");



    private void Awake()
    {
        instance = this;
    }

    #region Game Start Menu
    public void OnLocalPlayBTN()
    {
        Debug.Log("Click OnLocalPlayBTN");
        anim.SetTrigger(inGameHash);
    }

    public void OnOnlinePlayBTN()
    {
        Debug.Log("Click OnOnlinePlayBTN");
        anim.SetTrigger(onlineMenuHash);
    }
    #endregion

    #region Online Menu
    public void OnOnlineHostBTN()
    {
        Debug.Log("Click OnOnlineHostBTN");
        anim.SetTrigger(hotsMenuHash);
    }

    public void OnOnlineConnectBTN()
    {
        Debug.Log("Click OnOnlineConnectBTN"); //wait for connecting to host

    }

    public void OnOnlineBackBTN()
    {
        Debug.Log("Click OnOnlineBackBTN");
        anim.SetTrigger(startMenu);
    }
    #endregion

    #region Host Menu
    public void OnHostBackBTN()
    {
        Debug.Log("Click OnOnlineBackBTN");
        anim.SetTrigger(onlineMenuHash);
    }
    #endregion
}
