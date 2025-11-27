using UnityEngine;
using TMPro;

// Dieses Skript benötigt eine TMP_InputField Komponente am selben GameObject.
[RequireComponent(typeof(TMP_InputField))]
public class PlayerNameInputUI : MonoBehaviour
{
    private TMP_InputField _inputField;

    void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
    }

    void Start()
    {
        // Fülle das Eingabefeld mit dem bereits gespeicherten Namen, falls vorhanden.
        if (PlayerProfile.instance != null)
        {
            _inputField.text = PlayerProfile.instance.PlayerName;
        }
    }

    // Diese Methode wird vom Speicher-Button aufgerufen.
    public void SubmitName()
    {
        if (PlayerProfile.instance != null)
        {
            PlayerProfile.instance.SetPlayerName(_inputField.text);
            Debug.Log($"Spielername auf '{_inputField.text}' gesetzt und gespeichert.");
        }
    }
}
