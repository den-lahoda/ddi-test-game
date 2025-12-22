using UnityEngine;
using System.Collections;

public class BossBehavior : MonoBehaviour
{
    [Header("Player & Trigger")]

    [SerializeField, Tooltip("Transform игрока. До него измеряется расстояние, чтобы запустить исчезновение.")]
    private Transform player;

    [SerializeField, Tooltip("Дистанция до игрока, при которой босс начинает исчезать.")]
    private float vanishDistance = 10f;


    [Header("Vanish Settings")]

    [SerializeField, Tooltip("На сколько юнитов сместить босса в сторону исчезновения во время эффекта.")]
    private float moveBackDistance = 3f;

    [SerializeField, Tooltip("Длительность растворения и смещения (в секундах).")]
    private float vanishDuration = 2f;


    [Header("Spawn Settings")]

    [SerializeField, Tooltip("Префаб нового босса, который появится после исчезновения. Можно оставить пустым.")]
    private GameObject bossPrefabToSpawn;


    private bool isVanishing = false;
    private Material bossMaterial;
    private Color startColor;
    private Vector3 vanishDirection;

    // Глобальный флаг: босс может появиться только один раз на сцене
    private static bool hasAppeared = false;

    void Start()
    {
        bossMaterial = GetComponent<Renderer>().material;
        startColor = bossMaterial.color;

        if (bossMaterial.HasProperty("_Color"))
        {
            bossMaterial.SetFloat("_Mode", 3);
            Color c = bossMaterial.color;
            c.a = 1f;
            bossMaterial.color = c;

            bossMaterial.EnableKeyword("_ALPHABLEND_ON");
            bossMaterial.renderQueue = 3000;
        }

        if (hasAppeared)
            gameObject.SetActive(false);
    }

    void Update()
    {
        if (isVanishing || hasAppeared || player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist < vanishDistance)
        {
            StartCoroutine(VanishEffect());
            isVanishing = true;
            hasAppeared = true;
        }
    }

    private IEnumerator VanishEffect()
    {
        vanishDirection = Vector3.left;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + vanishDirection * moveBackDistance;

        float elapsed = 0f;

        while (elapsed < vanishDuration)
        {
            float t = elapsed / vanishDuration;

            transform.position = Vector3.Lerp(startPos, targetPos, t);

            float alpha = Mathf.Lerp(1f, 0f, t);
            bossMaterial.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);

        if (bossPrefabToSpawn != null)
            Instantiate(bossPrefabToSpawn, transform.position, Quaternion.identity);
    }
}
