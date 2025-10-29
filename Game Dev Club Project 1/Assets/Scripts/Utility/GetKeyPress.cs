using System.Collections.Generic;
using UnityEngine;

public class GetKeyPress : MonoBehaviour
{
    private HashSet<KeyCode> keysToCheck = new HashSet<KeyCode>();
    private TextDisplay playerInputDisplayer;

    void Awake()
    {
        for (KeyCode k = KeyCode.A; k <= KeyCode.Z; k++) keysToCheck.Add(k);
        for (KeyCode k = KeyCode.Alpha0; k <= KeyCode.Alpha9; k++) keysToCheck.Add(k);
        keysToCheck.Add(KeyCode.UpArrow);
        keysToCheck.Add(KeyCode.DownArrow);
        keysToCheck.Add(KeyCode.LeftArrow);
        keysToCheck.Add(KeyCode.RightArrow);
        keysToCheck.Add(KeyCode.Space);
        keysToCheck.Add(KeyCode.Return);
        keysToCheck.Add(KeyCode.Escape);
    }

    void Start()
    {
    playerInputDisplayer = TextDisplayManager.NewUI(new Vector3(-700f, -400f, 0f), 1f)
        .WithTrackedProvider(GetLatestKeyPressed)
            .WithDraggable()
            .WithInitialText("Waiting for Input")
            .Build();
    }

    public string GetLatestKeyPressed()
    {
        foreach (KeyCode key in keysToCheck)
        {
            if (Input.GetKey(key))
            {
                return key.ToString();
            }
        }

        return "";
    }
}
