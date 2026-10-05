using UnityEngine;

public abstract class AttackDataSO : ScriptableObject
{
    [Header("Properties")]
    public int damage = 1;
    public int Enemycount;

    [Header("Player Animations")]
    public AnimationClip ChargeAnim;
    public AnimationClip AttackAnim;

    [Header("VFX")]
    public GameObject AttackVFX;
    public GameObject ChargeVFX;

    [Header("Audio")]
    public AudioClip attackSFX;
}