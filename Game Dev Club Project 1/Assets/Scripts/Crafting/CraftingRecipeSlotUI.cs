using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingRecipeSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private CraftingRecipeClass recipe;
    private Button button;
    private Image recipeIcon;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("craft");
        CraftingManager.Instance.SelectRecipe(recipe);
        //change color
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //CraftingManager.Instance.SelectRecipe(recipe);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRecipe(CraftingRecipeClass recipe)
    {
        this.recipe = recipe;
    }

    public CraftingRecipeClass GetRecipe()
    {
        return recipe;
    }
}
