using UnityEngine;
using System.Threading.Tasks;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameObject[] InitializeObjectList;

    async void Awake()
    {
        if (InitializeObjectList == null) return;

        foreach (GameObject obj in InitializeObjectList)
        {
            if (obj == null) continue;

            // Instantiate the object
            Instantiate(obj);

            // Wait a frame to allow the instantiated object's Awake() to run
            await Task.Yield();
        }
    }
}