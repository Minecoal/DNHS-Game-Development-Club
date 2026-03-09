using UnityEngine;

[CreateAssetMenu(fileName = "new Tool Class", menuName = "Item/Tool")]
public class ToolClass : ItemClass
{
    [Header("Tool")]
    public ToolType toolType;
    public enum ToolType
    {
        Weapon,
        Pickaxe,
        Axe
    }
    public int dmg;

    public override ToolClass GetTool() { return this; }
}