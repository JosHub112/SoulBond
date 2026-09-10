using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoulAttackSO", menuName = "Scriptable Objects/SoulAttackSO")]
public class SoulAttackSO : ScriptableObject
{
    [Header("Properties")]
    public int damage = 1;
    public int Enemycount;

    [Header("Player Animations")]
    public AnimationClip ChargeAnim;
    public AnimationClip AttackAnim;

    [Header("Soul Animation")]
    public AnimationClip SoulChargeAnim;
    public AnimationClip SoulAttackAnim;

    [Header("VFX")]
    public GameObject AttackVFX;
    public GameObject ChargeVFX;

}
