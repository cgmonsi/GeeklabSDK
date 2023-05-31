using UnityEngine;
using System.Collections;

public class MetricToggle : MonoBehaviour
{
    private bool dataCollectionActive;

    public void InitializeMetrics()
    {
        StartCoroutine(CheckDataCollectionStatus());
    }

    private IEnumerator CheckDataCollectionStatus()
    {
        while (true)
        {
            WebRequestManager.Instance.CheckDataCollectionStatusRequest(
                (response) => {
                    Debug.Log("Success: " + response);
                    dataCollectionActive = bool.Parse(response);
                },
                (error) => {
                    Debug.LogError("Error: " + error);
                }
            );
            
            
            yield return new WaitForSeconds(5);
        }
    }

    public bool IsDataCollectionActive()
    {
        return dataCollectionActive;
    }
}