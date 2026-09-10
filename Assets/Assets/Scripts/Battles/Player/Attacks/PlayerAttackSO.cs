using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttackSO", menuName = "Scriptable Objects/PlayerAttackSO")]
public class PlayerAttackSO : ScriptableObject
{
    [Header("Properties")]
    public int damage = 1;
    public int Enemycount;

    [Header("Animations")]
    public AnimationClip ChargeAnim;
    public AnimationClip AttackAnim;

    [Header("VFX")]
    public GameObject AttackVFX;
    public GameObject ChargeVFX;
    
}
