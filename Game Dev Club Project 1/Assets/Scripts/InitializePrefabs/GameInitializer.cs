using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameObject[] InitializeObjectList;

    void Awake()
    {
        if (InitializeObjectList == null) return;
        foreach (GameObject _object in InitializeObjectList)
        {
            if (_object == null) continue;
            Instantiate(_object);
        }
    }
}
