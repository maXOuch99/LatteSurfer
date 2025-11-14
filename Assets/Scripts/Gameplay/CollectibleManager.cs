using UnityEngine;
using TMPro;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance;

    [Header("Score")]
    public int score = 0;
    public TMP_Text scoreText; //assign TextMeshPro UI

    [Header("Pickup Feedback")]
    public AudioClip pickupSfx;
    public ParticleSystem pickupVfxPrefab;

    AudioSource audioSource;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
        PlayPickupFeedback();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Sugar Cubes: " + score;
    }

    void PlayPickupFeedback()
    {
        if (pickupSfx != null)
            audioSource.PlayOneShot(pickupSfx);

        if (pickupVfxPrefab != null)
        {
            //instantiate particle at random point or camera; simple approach:
            Instantiate(pickupVfxPrefab, Camera.main.transform.position + Camera.main.transform.forward * 1.5f, Quaternion.identity);
            //or instantiate at the pickup position in Collectible.OnPicked if you want exact location
        }
    }
}
