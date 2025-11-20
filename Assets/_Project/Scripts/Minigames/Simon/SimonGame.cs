using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SimonGame : MonoBehaviour
{
    [Header("Spiel-Elemente")]
    [Tooltip("Die vier klickbaren Buttons für das Spiel.")]
    [SerializeField] private Button[] colorButtons;
    [Tooltip("Text, um den Spielstatus oder den Punktestand anzuzeigen.")]
    [SerializeField] private TextMeshProUGUI statusText;
    [Tooltip("Button, um das Spiel zu starten oder neu zu starten.")]
    [SerializeField] private Button startButton;

    [Header("Spiel-Einstellungen")]
    [Tooltip("Die Geschwindigkeit, mit der die Sequenz angezeigt wird (in Sekunden).")]
    //[SerializeField] private float sequenceDisplaySpeed = 0.75f;
    [SerializeField] private float initialSequenceDisplaySpeed = 0.75f;
    [Tooltip("Der Faktor, um den die Geschwindigkeit pro Runde erhöht wird (z.B. 0.95 für 5% schneller).")]
    [SerializeField] private float speedIncreaseFactor = 0.95f;
    [Tooltip("Die maximale Geschwindigkeit (minimale Anzeigedauer in Sekunden).")]
    [SerializeField] private float minSequenceDisplaySpeed = 0.3f;
    [Tooltip("Die Dauer, die ein Button beim Drücken durch den Spieler aufleuchtet.")]
    [SerializeField] private float playerHighlightDuration = 0.2f;
    [Tooltip("Die Farbe, die ein Button annimmt, wenn er aufleuchtet.")]
    [SerializeField] private Color highlightColor = new Color(0.88f, 0.88f, 0.88f); // Hex: #E0E0E0


    private float currentSequenceDisplaySpeed;
    private List<int> sequence = new List<int>();
    private int playerInputIndex;
    private bool isPlayerTurn = false;
    private bool gameIsActive = false;

    private Color[] originalColors;

    private void Start()
    {
        // Speichere die Originalfarben der Buttons
        originalColors = new Color[colorButtons.Length];
        for (int i = 0; i < colorButtons.Length; i++)
        {
            originalColors[i] = colorButtons[i].GetComponent<Image>().color;
        }

        startButton.onClick.AddListener(StartGame);

        // Füge Listener zu den Farbtasten hinzu
        for (int i = 0; i < colorButtons.Length; i++)
        {
            int index = i; // Wichtig für den Lambda-Ausdruck
            colorButtons[i].onClick.AddListener(() => OnColorButtonPressed(index));
        }

        PrepareInitialState();
    }

    private void PrepareInitialState()
    {
        statusText.text = "Klicke Start!";
        startButton.gameObject.SetActive(true);
        SetButtonsInteractable(false);
        gameIsActive = false;
    }

    private void StartGame()
    {
        gameIsActive = true;
        startButton.gameObject.SetActive(false);
        sequence.Clear();
        currentSequenceDisplaySpeed = initialSequenceDisplaySpeed;
        StartCoroutine(ComputerTurn());
    }

    private IEnumerator ComputerTurn()
    {
        statusText.text = "Merk dir die Sequenz...";
        isPlayerTurn = false;
        SetButtonsInteractable(false);

        yield return new WaitForSeconds(1f);

        // Erhöhe die Geschwindigkeit für die neue Runde (außer in der ersten Runde)
        if (sequence.Count > 0)
        {
            currentSequenceDisplaySpeed = Mathf.Max(minSequenceDisplaySpeed, currentSequenceDisplaySpeed * speedIncreaseFactor);
        }

        // Füge ein neues, zufälliges Element zur Sequenz hinzu
        sequence.Add(Random.Range(0, colorButtons.Length));

        // Zeige die gesamte Sequenz an
        foreach (int index in sequence)
        {
            //yield return StartCoroutine(HighlightButton(index));
            //yield return new WaitForSeconds(sequenceDisplaySpeed / 2);
            yield return StartCoroutine(HighlightButton(index, currentSequenceDisplaySpeed));
            yield return new WaitForSeconds(currentSequenceDisplaySpeed / 2);
        }

        // Starte den Zug des Spielers
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        isPlayerTurn = true;
        playerInputIndex = 0;
        SetButtonsInteractable(true);
        statusText.text = "Du bist dran!";
    }

    private void OnColorButtonPressed(int index)
    {
        if (!isPlayerTurn) return;

        //StartCoroutine(HighlightButton(index));
        StartCoroutine(HighlightButton(index, playerHighlightDuration));

        if (sequence[playerInputIndex] == index)
        {
            playerInputIndex++;
            // Wenn die Sequenz korrekt vervollständigt wurde
            if (playerInputIndex >= sequence.Count)
            {
                // Gewinnbedingung prüfen: Punktzahl 10 erreicht
                if (sequence.Count == 10)
                {
                    WinGame();
                }
                else
                {
                    statusText.text = "Richtig!";
                    StartCoroutine(ComputerTurn());
                }
            }
        }
        else
        {
            // Falsche Eingabe
            GameOver();
        }
    }

    private void WinGame()
    {
        gameIsActive = false;
        isPlayerTurn = false;
        SetButtonsInteractable(false);
        statusText.text = "Gewonnen! Geniale Merkrate!";
        // Starte die Rückkehr zur Hauptszene
        StartCoroutine(ReturnToMainGameAfterDelay());
    }

    private void GameOver()
    {
        gameIsActive = false;
        isPlayerTurn = false;
        SetButtonsInteractable(false);
        statusText.text = $"Spiel vorbei! Deine Punktzahl: {sequence.Count - 1}";

        // Starte die Rückkehr zur Hauptszene
        StartCoroutine(ReturnToMainGameAfterDelay());
    }

    private IEnumerator HighlightButton(int index, float duration)
    {
        Image buttonImage = colorButtons[index].GetComponent<Image>();
        buttonImage.color = highlightColor;
        // Hier könnte man auch einen Sound abspielen
        //yield return new WaitForSeconds(sequenceDisplaySpeed);
        yield return new WaitForSeconds(duration);
        buttonImage.color = originalColors[index];
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Button button in colorButtons)
        {
            button.interactable = interactable;
        }
    }

    private IEnumerator ReturnToMainGameAfterDelay()
    {
        // Warte 3 Sekunden, damit der Spieler die Gewinn-/Verlustnachricht lesen kann
        yield return new WaitForSeconds(3f);
        if (SceneController.instance != null)
        {
            SceneController.instance.ReturnToMainGame();
        }
        else
        {
            Debug.LogWarning("SceneController nicht gefunden. Rückkehr zum Hauptspiel nicht möglich.");
        }
    }
}