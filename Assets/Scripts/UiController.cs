using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    [SerializeField] private GameObject[] Hearts;
    [SerializeField] private Text Text;
    [SerializeField] private Text MainText;
    [SerializeField] private Text RestartButtonText;
    [SerializeField] private GameObject GameOverWindow;
    [SerializeField] private Text HighScoreTextValue;

    private const string GameOverText = "G A M E  O V E R";

    private const string GameOverButtonText = "Retry";


    private void Start()
    {
        PlayerController.PlayerScoreChanged += OnPlayerScoreChanged;
        PlayerController.PlayerLifeAmountChanged += OnPlayerLifeAmountChanged;
        EventManager.Instance.AddListener(EventType.PlayerDied, OnPlayerDie);
    }

    private void OnPlayerDie(EventType eventType, Component sender, object param)
    {
        HighScoreTextValue.text = ((int) param).ToString();
        MainText.text = GameOverText;
        RestartButtonText.text = GameOverButtonText;
        GameOverWindow.SetActive(true);
    }

    private void OnPlayerLifeAmountChanged(int newLifeAmount)
    {
        foreach (var heartGO in Hearts)
        {
            heartGO.SetActive(false);
        }
        for (var i = 0; i < newLifeAmount; i++)
        {
            Hearts[i].SetActive(true);
        }
    }

    private void OnPlayerScoreChanged(int newScore)
    {
        Text.text = newScore.ToString();
    }

    public void OnRetryButtonPressed()
    {
        GameOverWindow.SetActive(false);
        EventManager.Instance.PostNotification(EventType.GameRestart, this);
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}
