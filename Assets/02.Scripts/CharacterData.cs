using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharDataSO", menuName ="Game/Character")]
public class CharacterData : ScriptableObject
{
    public string charName;
    public string charDescription;
    public int charID;
    public int charHP;
    public int charJumpCount;
    public float charMagnetRadius;
}
