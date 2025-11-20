using UnityEngine;
using UnityEngine.UI;
using TMPro; // Wichtig für TextMeshPro
using System.Collections;

namespace Minigames.RockPaperScissors
{
    // Definiert die möglichen Züge im Spiel.
    public enum Choice
    {
        Schere,
        Stein,
        Papier
    }

    // Steuert die Logik für das Schere-Stein-Papier Minispiel.
    public class RockPaperScissorsGame : MonoBehaviour
    {
        [Header("UI-Elemente")]
        [SerializeField] private TextMeshProUGUI resultText; // Umbenannt von resultText für Klarheit
        [SerializeField] private TextMeshProUGUI playerScoreText;
        [SerializeField] private TextMeshProUGUI computerScoreText;
        [SerializeField] private Image playerChoiceImage;
        [SerializeField] private Image computerChoiceImage;
        [SerializeField] private Button[] choiceButtons;

        [Header("Grafiken für die Auswahl")]
        [SerializeField] private Sprite schereSprite;
        [SerializeField] private Sprite steinSprite;
        [SerializeField] private Sprite papierSprite;

        [Header("Spiel-Einstellungen")]
        [SerializeField] private int scoreToWin = 3;

        private int playerScore;
        private int computerScore;
        private bool isRoundOver = false;

        private void Start()
        {
            playerScore = 0;
            computerScore = 0;
            ResetRound();
        }

        // Wird aufgerufen, wenn der Spieler eine Auswahl trifft (über einen Button).
        // playerChoiceInt: Die Auswahl des Spielers als Integer (0=Schere, 1=Stein, 2=Papier).
        public void OnPlayerChoice(int playerChoiceInt)
        {
            if (isRoundOver) return;

            Choice playerChoice = (Choice)playerChoiceInt;

            // Deaktiviere die Auswahl-Buttons, um weitere Eingaben zu verhindern
            SetChoiceButtonsInteractable(false);

            // Computer trifft eine zufällige Auswahl
            Choice computerChoice = (Choice)Random.Range(0, 3);

            // Zeige die Auswahl an
            DisplayChoices(playerChoice, computerChoice);

            // Bestimme das Ergebnis
            DetermineWinner(playerChoice, computerChoice);

            // Starte die nächste Runde oder beende das Spiel
            StartCoroutine(EndRound());
        }

        // Setzt die Runde in den Anfangszustand zurück.
        private void ResetRound()
        {
            isRoundOver = false;
            resultText.text = "Wähle deine Hand!";
            playerChoiceImage.gameObject.SetActive(false);
            computerChoiceImage.gameObject.SetActive(false);
            SetChoiceButtonsInteractable(true);
            UpdateScoreDisplay();
        }

        // Aktiviert oder deaktiviert alle Auswahl-Buttons.
        private void SetChoiceButtonsInteractable(bool isInteractable)
        {
            foreach (var button in choiceButtons)
            {
                button.interactable = isInteractable;
            }
        }

        // Zeigt die Grafiken für die Auswahl des Spielers und des Computers an.
        private void DisplayChoices(Choice player, Choice computer)
        {
            playerChoiceImage.sprite = GetSpriteForChoice(player);
            computerChoiceImage.sprite = GetSpriteForChoice(computer);

            playerChoiceImage.gameObject.SetActive(true);
            computerChoiceImage.gameObject.SetActive(true);
        }

        // Ermittelt den Gewinner und aktualisiert den Ergebnis-Text.
        private void DetermineWinner(Choice player, Choice computer)
        {
            if (player == computer)
            {
                resultText.text = "Unentschieden!";
                return;
            }

            bool playerWins = (player == Choice.Schere && computer == Choice.Papier) ||
                              (player == Choice.Stein && computer == Choice.Schere) ||
                              (player == Choice.Papier && computer == Choice.Stein);
            
            if (playerWins)
            {
                resultText.text = "Runde gewonnen!";
                playerScore++;
            }
            else
            {
                resultText.text = "Runde verloren!";
                computerScore++;
            }

            UpdateScoreDisplay();
        }

        private void UpdateScoreDisplay()
        {
            playerScoreText.text = $"YOU: {playerScore}";
            computerScoreText.text = $"CPU: {computerScore}";
        }

        private IEnumerator EndRound()
        {
            isRoundOver = true;
            yield return new WaitForSeconds(2f); // Kurze Pause, um das Ergebnis zu lesen

            if (playerScore >= scoreToWin)
            {
                EndGame(true);
            }
            else if (computerScore >= scoreToWin)
            {
                EndGame(false);
            }
            else
            {
                ResetRound();
            }
        }

        private void EndGame(bool playerHasWon)
        {
            SetChoiceButtonsInteractable(false);
            if (playerHasWon)
            {
                // Sage dem SceneController, dass das Minispiel erfolgreich war.
                if (SceneController.instance != null)
                {
                    SceneController.instance.CompleteCurrentMinigame();
                }
                resultText.text = "Sieg! Du hast das Spiel gewonnen!";
            }
            else
            {
                resultText.text = "Verloren! Der Computer hat gewonnen.";
            }
            
            // Starte die Coroutine, um nach einer kurzen Verzögerung zurückzukehren.
            StartCoroutine(ReturnToMainGameAfterDelay());
        }
        
        private IEnumerator ReturnToMainGameAfterDelay()
        {
            // Warte 3 Sekunden, damit der Spieler die Nachricht lesen kann.
            yield return new WaitForSeconds(3f);
            if (SceneController.instance != null)
            {
                SceneController.instance.ReturnToMainGame();
            }
        }
        
        // Gibt das passende Sprite für eine gegebene Auswahl zurück.
        private Sprite GetSpriteForChoice(Choice choice)
        {
            switch (choice)
            {
                case Choice.Schere: return schereSprite;
                case Choice.Stein:  return steinSprite;
                case Choice.Papier: return papierSprite;
                default:            return null;
            }
        }
    }
}
