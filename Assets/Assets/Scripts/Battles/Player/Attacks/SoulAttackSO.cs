using UnityEngine;

[CreateAssetMenu(fileName = "SoulAttackSO", menuName = "Scriptable Objects/SoulAttackSO")]
public class SoulAttackSO : AttackDataSO
{
    [Header("Soul Animation")]
    public AnimationClip SoulChargeAnim;
    public AnimationClip SoulAttackAnim;
}