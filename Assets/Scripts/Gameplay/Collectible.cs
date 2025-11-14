using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible")]
    public int value = 1;
    public float respawnDelay = 0f;    // set >0 if you want respawn
    public bool usePooling = false;    // optional for later

    [Header("Polish")]
    public float bobAmplitude = 0.12f;
    public float bobFrequency = 1.5f;
    public float spinSpeed = 90f; // degrees per second

    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // bob + spin polish
        transform.localPosition = startPos + Vector3.up * Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectibleManager.Instance.AddScore(value);
            OnPicked(other.gameObject);
        }
    }

    void OnPicked(GameObject picker)
    {
        // play pickup VFX / SFX here (see CollectibleManager below)
        // for now just destroy or disable
        if (usePooling)
        {
            gameObject.SetActive(false); // replace later with pool return
        }
        else if (respawnDelay > 0f)
        {
            StartCoroutine(RespawnCoroutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    System.Collections.IEnumerator RespawnCoroutine()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        gameObject.SetActive(true);
    }
}
