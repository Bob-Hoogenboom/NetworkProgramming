using TMPro;
using UnityEngine;

/// <summary>
/// Sample ResultUI, can be changed or connected to a json or save string later
/// </summary>
public class ResultUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject resultWindow;
    [SerializeField] private TMP_Text whiteScoreTXT;
    [SerializeField] private TMP_Text blackScoreTXT;
    [Space]
    [SerializeField] private int whiteScore = 0;
    [SerializeField] private int blackScore = 0;

    private void OnEnable()
    {
        GameEvents.gameOverEvent += OnGameOver;
        GameEvents.rematchEvent += Rematch;
    }

    private void OnDisable()
    {
        GameEvents.gameOverEvent -= OnGameOver;
        GameEvents.rematchEvent -= Rematch;
    }

    private void Start()
    {
        resultWindow.SetActive(false);
    }

    private void OnGameOver(int winningTeam)
    {
        resultWindow.SetActive(true);

        if (winningTeam == 0)
        {
            whiteScore++;
            DisplayWinning(0);

            //if you are white display that YOU have won

            Debug.Log("White wins! White Score: " + whiteScore);
        }
        else if (winningTeam == 1)
        {
            blackScore++;
            DisplayWinning(1);

            //if you are black display that YOU have won

            Debug.Log("Black wins! Black Score: " + blackScore);
        }
    }

    private void DisplayWinning(int Winner)
    {
        if(Winner == 0)
        {
            whiteScoreTXT.text = $"White Won This Round!\nScore: {whiteScore}";
            blackScoreTXT.text = $"Black Score:\n{blackScore}";

        }
        else
        {
            whiteScoreTXT.text = $"White Score:\n{whiteScore}";
            blackScoreTXT.text = $"Black Won This Round!\nScore: {blackScore}";
        }

    }

    public void ResetGame()
    {
        ChessBoard board = FindObjectOfType<ChessBoard>();
        board.OnRematch();
        board.ResetBoard();
        resultWindow.SetActive (false);
    }

    public void ExitGame()
    {
        ChessBoard board = FindObjectOfType<ChessBoard>();
        board.ToMenu();
    }

    public void Rematch()
    {

    }

}