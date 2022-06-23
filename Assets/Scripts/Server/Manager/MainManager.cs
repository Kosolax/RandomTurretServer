using UnityEngine;

using Zenject;

public class MainManager : MonoBehaviour
{
    [Inject]
    private readonly ServerBusiness serverBusiness;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        this.serverBusiness.Start(50, 8000);
    }
}