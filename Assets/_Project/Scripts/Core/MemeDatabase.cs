using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MemeDatabase", menuName = "Memes/Meme Database")]
public class MemeDatabase : ScriptableObject
{
    public List<MemeData> allMemes;
}
