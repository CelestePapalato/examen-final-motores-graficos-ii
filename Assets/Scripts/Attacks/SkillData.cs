using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "ScriptableObjects/Skill Data", order = 3)]
public class SkillData : ScriptableObject
{
    public int Damage;
    public string AnimationTrigger;
    public GameObject SpawnableObject;

    public void SetupCharacter(Character character)
    {
        foreach(Damage damage in character.DamageComponents)
        {
            damage.DamagePoints = Damage;
        }
    }
}
