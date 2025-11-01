# **Pocket Pixel Party Gem**

## **Name**

Pocket Pixel Party

## **Beschreibung**

"Pocket Pixel Party" ist ein innovatives, reines Singleplayer-Hybridspiel, das die präzise Steuerung klassischer Jump 'n' Run-Titel (à la Super Mario) mit der zufälligen, kompetitiven Herausforderung kurzer Party-Minispiele (à la Mario Party) vereint.

Das Spiel wird in der Unity Engine entwickelt und ist ausschließlich für Desktop-Plattformen (Windows .exe) konzipiert, um eine hohe Performance zu gewährleisten. Die visuelle Ästhetik ist eine charmante, durchgängige 16-Bit-Pixel-Art-Optik.

Das Hauptziel ist die Vervollständigung einer umfassenden Sammlung von Meme-Bildern, die man durch das Besiegen eines Computergegners in den zufällig ausgewählten Minispielen verdient.

## **Anleitung**

Die Anforderungen und Kern-Gameplay-Elemente sind wie folgt:

### **1\. Avatar-Auswahl und Steuerung**

* Der Spieler wählt einen kosmetischen Avatar im Pixel-Art-Look.  
* Die Steuerung erfolgt präzise über Tastatur, optimiert für Jump 'n' Run.

### **2\. Kern-Gameplay-Loop**

* **Lauf und Erkundung:** Steuere den Avatar durch thematische Jump 'n' Run-Level, weiche Hindernissen aus und meistere präzise Sprünge.  
* **Meme-Herausforderung:** Interagiere mit einem "Meme-Symbol" im Level, um das Gameplay nahtlos zu unterbrechen und in die Minispiel-Arena zu wechseln.

### **3\. Minispiele (Zufallsauswahl gegen KI)**

Die Minispiele sind kurz, intensiv und erfordern unterschiedliche Fähigkeiten. Die Kategorien umfassen:

* **Strategie & Logik:** 4 Gewinnt, TicTacToe, Minesweeper, Puzzle.  
* **Geschicklichkeit & Reflexe:** Flappy Bird (Variante), Doodle Jump (Variante), Pinball (Variante).  
* **Gedächtnis & Tempo:** Klick-Spiel (Reihenfolge merken), Memory.  
* **Glück & Raten:** Hangman, Schere, Stein, Papier, Quiz.

### **4\. Progression und Speicherung**

* **Meme-Freischaltung:** Ein gewonnener Minispiel-Sieg schaltet das Meme frei und fügt es zur Galerie (Achievement-Baum) hinzu.  
* **Dynamischer Schwierigkeitsanstieg:** Mit jedem gesammelten Meme steigt kontinuierlich der Schwierigkeitsgrad der Computergegner in den Minispielen und die Jump 'n' Run-Level werden durch neue Hindernisse anspruchsvoller.  
* **Belohnungen:** Das Sammeln vollständiger Meme-Sets schaltet zusätzliche kosmetische Belohnungen (z. B. Avatar-Skins oder Effekte) frei.  
* **Datenspeicherung:** Der gesamte Fortschritt und die Scores werden lokal mittels **SQLite** gespeichert.

### **5\. Deployment**

* Das finale Spiel wird als eigenständiger Build (PC, Zielsystem Windows) erstellt.  
* Abgabe erfolgt als komprimiertes Verzeichnis (.zip/.rar), das die lauffähige .exe-Datei enthält.
