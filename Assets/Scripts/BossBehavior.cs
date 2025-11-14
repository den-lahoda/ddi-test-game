using UnityEngine;
using System.Collections;

public class BossBehavior : MonoBehaviour
{
    [Header("Player & Trigger")]
    [SerializeField] private Transform player;
    [SerializeField] private float vanishDistance = 10f;

    [Header("Vanish Settings")]
    [SerializeField] private float moveBackDistance = 3f;
    [SerializeField] private float vanishDuration = 2f;

    [Header("Spawn Chasing Boss")]
    [SerializeField] private GameObject chasingBossPrefab;

    private bool isVanishing = false;
    private Material bossMaterial;
    private Color startColor;
    private Vector3 vanishDirection;

    // Контроль единственного появления для всей сцены
    private static bool hasAppeared = false;

    void Start()
    {
        // Получаем материал и включаем поддержку прозрачности
        bossMaterial = GetComponent<Renderer>().material;
        startColor = bossMaterial.color;

        if (bossMaterial.HasProperty("_Color"))
        {
            // Делаем материал прозрачным, чтобы alpha работала
            bossMaterial.SetFloat("_Mode", 3); // Standard shader: 3 = Transparent
            Color c = bossMaterial.color;
            c.a = 1f;
            bossMaterial.color = c;
            bossMaterial.EnableKeyword("_ALPHABLEND_ON");
            bossMaterial.renderQueue = 3000;
        }

        // Если босс уже появлялся, сразу скрываем
        if (hasAppeared)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isVanishing || hasAppeared) return;

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
        // фиксированное направление влево
        vanishDirection = Vector3.left; 
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + vanishDirection * moveBackDistance;

        float elapsed = 0f;

        while (elapsed < vanishDuration)
        {
            float t = elapsed / vanishDuration;

            // Плавное движение
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            // Плавное растворение
            float alpha = Mathf.Lerp(1f, 0f, t);
            bossMaterial.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        // Спавним второго босса
        if (chasingBossPrefab != null)
        {
            GameObject newBoss = Instantiate(chasingBossPrefab);
            ChasingBoss cb = newBoss.GetComponent<ChasingBoss>();
            if (cb != null)
            {
                cb.SetPlayer(player); // передадим ссылку на игрока
            }
        }

    }

}
