using UnityEngine;

[CreateAssetMenu(menuName = "Animations/Animation List")]
public class AnimationList : ScriptableObject
{
    public RuntimeAnimatorController controllerReference;
    public AnimationEntry[] entries;
}

[System.Serializable]
public struct AnimationEntry
{
    public AnimationID id;
    public string name;
    public int hash;
}