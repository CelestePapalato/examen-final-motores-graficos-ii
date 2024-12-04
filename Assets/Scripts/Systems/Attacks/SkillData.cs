using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "ScriptableObjects/Skill Data", order = 3)]
public class SkillData : ScriptableObject
{
    public int Damage;
    public int ManaPoints;
    public float OnStartImpulse;
    public string AnimationTrigger;
    public Projectile Projectile;

    public void SetupCharacter(Character character)
    {
        foreach(Damage damage in character.DamageComponents)
        {
            damage.DamagePoints = Damage;
        }
        ProjectileShooter shooter = character.Shooter;
        if (shooter) { shooter.Projectile = Projectile; }
    }
}
