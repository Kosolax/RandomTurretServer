using UnityEngine;

public class HelperManager : MonoBehaviour
{
    public static HelperManager Instance { get; set; }

    public void DestroyGameObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }

    public GameObject InstantiateGameObject(GameObject prefabToInstantiate, Transform parentTransform)
    {
        GameObject gameObject = Instantiate(prefabToInstantiate);
        gameObject.transform.SetParent(parentTransform);
        gameObject.transform.localPosition = new Vector3(0, 0, 0);

        return gameObject;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
}