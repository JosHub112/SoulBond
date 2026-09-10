using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoulSO", menuName = "Scriptable Objects/SoulSO")]
public class SoulSO : ScriptableObject
{
    public AnimationClip SoulIdle;
    public GameObject SoulSprite;


    public List<SoulAttackSO> attacks = new List<SoulAttackSO>();

}
