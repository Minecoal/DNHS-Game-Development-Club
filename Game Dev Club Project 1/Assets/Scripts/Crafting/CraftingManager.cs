using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    private List<CraftingRecipeClass> recipes = new List<CraftingRecipeClass>();
    [SerializeField] private GameObject recipePrefab;
    [SerializeField] private GameObject recipeHolder;

    [SerializeField] private Transform craftInfoHolder;
    [SerializeField] private Transform craftMaterialInfoHolder;
    [SerializeField] private GameObject craftMaterialInfoPrefab;

    [SerializeField] private Button craftButton;

    [SerializeField] private Slider amountSlider;
    private TMPro.TextMeshProUGUI minAmountText;
    private TMPro.TextMeshProUGUI maxAmountText;


    private List<CraftingRecipeClass> craftableRecipes = new List<CraftingRecipeClass>();
    private List<CraftingRecipeClass> uncraftableRecipes = new List<CraftingRecipeClass>();

    private CraftingRecipeClass selectedRecipe;
    private int mostCraftableAmount = 0;
    private int numberToCraft = 1;

    [SerializeField] private GameObject craftingUICanvas; //Canvas
    private bool previousCursorState;
    private bool isCraftingMenuOpen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Disable when parented to a DonDestoryOnLoad object
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minAmountText = craftInfoHolder.Find("Min Amount Craftable").GetComponent<TMPro.TextMeshProUGUI>();
        maxAmountText = craftInfoHolder.Find("Max Amount Craftable").GetComponent<TMPro.TextMeshProUGUI>();

        recipes = Resources.LoadAll<CraftingRecipeClass>("CraftingRecipes").ToList();

        LoadRecipes();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {

            LoadRecipes();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCraftingMenu();
        }
    }

    public void Craft()    
    {
        if (selectedRecipe.CanCraft())
        {
            selectedRecipe.Craft(numberToCraft);
            LoadRecipes();
            SelectRecipe(selectedRecipe); //optimize this later
        }
        else
        {
            Debug.Log("cannot craft");
        }
    }

    public void SelectRecipe(CraftingRecipeClass recipe)
    {
        selectedRecipe = recipe;

        for (int i = craftMaterialInfoHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(craftMaterialInfoHolder.GetChild(i).gameObject);
        }

        SetSlider(recipe);

        

        craftInfoHolder.Find("Recipe Output Image").GetComponent<Image>().sprite = recipe.outputItem.GetItem().itemIcon;
        craftInfoHolder.Find("Output Amount Text").GetComponent<TMPro.TextMeshProUGUI>().text = (recipe.outputItem.GetQuantity() * numberToCraft).ToString();

        foreach (ItemSlot input in recipe.inputItems)
        {
            GameObject material = Instantiate(craftMaterialInfoPrefab, craftMaterialInfoHolder, false);

            material.transform.Find("Image").GetComponent<Image>().sprite = input.GetItem().itemIcon;

            string textColorTag = InventoryManager.Instance.ContainAmount(input.GetItem()) >= (input.GetQuantity() * numberToCraft) ? "<color=#04bf97>" : "<color=#FF0000>";
            material.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = textColorTag + InventoryManager.Instance.ContainAmount(input.GetItem()) + "</color> / " + (input.GetQuantity() * numberToCraft);

        }

        craftButton.interactable = recipe.CanCraft();
    }

    public void LoadRecipes()
    {
        craftableRecipes.Clear();
        uncraftableRecipes.Clear();

        amountSlider.value = 1;

        for (int i = recipeHolder.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(recipeHolder.transform.GetChild(i).gameObject);
        }

        foreach (CraftingRecipeClass recipe in recipes)
        {
            if (recipe.CanCraft())
            {
                craftableRecipes.Add(recipe);
            }
            else
            {
                uncraftableRecipes.Add(recipe);
            }

        }

        foreach (CraftingRecipeClass recipe in craftableRecipes)
        {
            GameObject recipeButton = Instantiate(recipePrefab, recipeHolder.transform, false);

            recipeButton.transform.Find("Image").GetComponent<Image>().sprite = recipe.outputItem.GetItem().itemIcon;
            recipeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = recipe.outputItem.GetItem().itemName;
            recipeButton.GetComponent<CraftingRecipeSlotUI>().SetRecipe(recipe);

        }

        foreach (CraftingRecipeClass recipe in uncraftableRecipes)
        {
            GameObject recipeButton = Instantiate(recipePrefab, recipeHolder.transform, false);

            recipeButton.transform.Find("Image").GetComponent<Image>().sprite = recipe.outputItem.GetItem().itemIcon;
            recipeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = recipe.outputItem.GetItem().itemName;
            recipeButton.GetComponent<CraftingRecipeSlotUI>().SetRecipe(recipe);
            recipeButton.GetComponent<Button>().interactable = false;

        }
    }

    public void RefreshCurrentRecipe()
    {
        amountSlider.value = 1;
    }

    private void SetSlider(CraftingRecipeClass recipe)
    {
        if (recipe.CanCraft())
        {
            amountSlider.gameObject.SetActive(true);
            minAmountText.gameObject.SetActive(true);
            maxAmountText.gameObject.SetActive(true);


            int tempMostAmountCraftable = int.MaxValue;

            foreach (ItemSlot input in recipe.inputItems)
            {
                int amountCraftable = Mathf.FloorToInt(InventoryManager.Instance.ContainAmount(input.GetItem()) / input.GetQuantity());
                if (amountCraftable < tempMostAmountCraftable)
                    tempMostAmountCraftable = amountCraftable;
            }

            mostCraftableAmount = tempMostAmountCraftable;

            if(mostCraftableAmount == 1)
            {
                amountSlider.maxValue = 2;
                amountSlider.interactable = false;
            }
            else
            {
                amountSlider.maxValue = mostCraftableAmount;
                amountSlider.interactable = true;
            }

            minAmountText.text = "1";
            maxAmountText.text = mostCraftableAmount.ToString();
        }
        else
        {
            mostCraftableAmount = 0;
            amountSlider.maxValue = 2;
            amountSlider.interactable = false;

            amountSlider.gameObject.SetActive(false);
            minAmountText.gameObject.SetActive(false);
            maxAmountText.gameObject.SetActive(false);
        }

    }

    public void OnSliderChange()
    {
        numberToCraft = (int)(amountSlider.value);
        SelectRecipe(selectedRecipe);
        
    }

    public void ToggleCraftingMenu()
    {
        if (isCraftingMenuOpen)
        {
            CloseCraftingMenu();
        }
        else
        {
            OpenCraftingMenu();
        }
    }

    public void CloseCraftingMenu()
    {
        isCraftingMenuOpen = false;
        craftingUICanvas?.SetActive(false);
        Cursor.visible = previousCursorState;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenCraftingMenu()
    {
        isCraftingMenuOpen = true;
        craftingUICanvas?.SetActive(true);
        previousCursorState = Cursor.visible;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        LoadRecipes();
    }
}
