using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    public GameObject hazardPrefab;
    public float spawnRate = 3f; // כל כמה שניות ייפול מטאור
    public float minX = -8f; // גבול שמאלי לשיגור
    public float maxX = 8f;  // גבול ימני לשיגור

    public AudioClip spawnSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        // קריאה לפונקציית השיגור בלולאה
        InvokeRepeating(nameof(SpawnHazard), 2f, spawnRate);
    }

    void SpawnHazard()
    {
        // מגרילים מיקום בציר ה-X
        float randomX = Random.Range(minX, maxX);
        Vector2 spawnPos = new Vector2(randomX, transform.position.y);
        
        // יוצרים את האסטרואיד
        Instantiate(hazardPrefab, spawnPos, hazardPrefab.transform.rotation);
        // משמיעים רעש יציאה
        if (spawnSound != null) audioSource.PlayOneShot(spawnSound);
    }
}