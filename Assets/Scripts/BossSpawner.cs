using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject boss;
    private static bool hasAppeared = false; // статическая переменная для всей сцены

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasAppeared)
        {
            boss.SetActive(true);
            hasAppeared = true; // теперь больше не появится
            Debug.Log("Босс появился!");
        }
    }
}
