using UnityEngine;
using Cinemachine; // Wichtig: Diese Zeile fuer die Kamera hinzufuegen!

public class CharacterApplier : MonoBehaviour
{
    void Start() // Awake() zu Start() aendern
    {
        // Finde das persistente CharacterSelection-Objekt
        CharacterSelection selectionInstance = CharacterSelection.Instance;

        // Pruefe, ob die Instanz und ein ausgewaehltes Prefab existieren
        if (selectionInstance != null && selectionInstance.selectedCharacterPrefab != null)
        {
            // Finde das existierende Spieler-Objekt in der Szene anhand seines Tags.
            // WICHTIG: Dein Spieler-Objekt in der SampleScene muss den Tag "Player" haben!
            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");

            if (existingPlayer != null)
            {
                // Finde die virtuelle Kamera in der Szene.
                CinemachineVirtualCamera virtualCamera = FindFirstObjectByType<CinemachineVirtualCamera>();

                // Erstelle eine neue Instanz des ausgewaehlten Charakters an der Position des alten Spielers.
                GameObject newPlayer = Instantiate(selectionInstance.selectedCharacterPrefab, existingPlayer.transform.position, existingPlayer.transform.rotation);
                newPlayer.name = selectionInstance.selectedCharacterPrefab.name; // Benenne das neue Objekt zur Klarheit

                // Zerstoere den alten Platzhalter-Spieler.
                Destroy(existingPlayer);

                // **DIE LOESUNG:** Weise der Kamera das neue Ziel zu.
                if (virtualCamera != null)
                {
                    virtualCamera.Follow = newPlayer.transform;
                }

                Debug.Log("Charakter " + newPlayer.name + " wurde im Spiel instanziiert.");
            }
            else
            {
                Debug.LogError("CharacterApplier konnte kein Objekt mit dem Tag 'Player' in der Szene finden!");
            }
        }
        else
        {
            Debug.Log("CharacterSelection.Instance nicht gefunden. Das ist normal, wenn die SampleScene direkt gestartet wird.");
        }
    }
}
