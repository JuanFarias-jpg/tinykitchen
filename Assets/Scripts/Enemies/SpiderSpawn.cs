using UnityEngine;

public class SpiderSpawn : MonoBehaviour
{
    [Header("Araña")]
    public GameObject spider;

    private void Start()
    {
        
        if (spider != null)
            spider.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (spider != null)
                spider.SetActive(true);
        }
    }
}