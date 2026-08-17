
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro; // השורה הזו היא הקסם שמאפשר לנו לעבוד עם הטקסטים המתקדמים!

// public class PlayerMovement : MonoBehaviour
// {
//     [Header("Movement Settings")]
//     public float moveSpeed = 8f;
//     private Vector2 targetPosition;
//     private bool isMoving = false;
//     private Collider2D playerCollider; 

//     private Vector2 currentDirection = Vector2.zero;
//     private Vector2 nextDirection = Vector2.zero;

//     [Header("UI Elements (Drag from Canvas)")]
//     public GameObject popupPanel;
    
//     // שינינו את הסוג ל-TextMeshProUGUI
//     public TextMeshProUGUI uiTitle; 
//     public TextMeshProUGUI uiDescription; 

//     void Start()
//     {
//         targetPosition = transform.position;
//         playerCollider = GetComponent<Collider2D>();
//     }

//     void Update()
//     {
//         if (Time.timeScale == 0f) return;

//         HandleInput();

//         if (!isMoving)
//         {
//             if (nextDirection != Vector2.zero)
//             {
//                 if (CanMove(nextDirection))
//                 {
//                     currentDirection = nextDirection;
//                     MoveTo(currentDirection);
//                 }
//                 else if (currentDirection != Vector2.zero && CanMove(currentDirection))
//                 {
//                     MoveTo(currentDirection);
//                 }
//             }
//         }
//         else
//         {
//             transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

//             if ((Vector2)transform.position == targetPosition)
//             {
//                 isMoving = false;
//             }
//         }
//     }

//     private void HandleInput()
//     {
//         float x = Input.GetAxisRaw("Horizontal");
//         float y = Input.GetAxisRaw("Vertical");

//         if (x == 0 && y == 0)
//         {
//             nextDirection = Vector2.zero;
//             currentDirection = Vector2.zero;
//         }
//         else
//         {
//             if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) nextDirection = Vector2.right;
//             if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) nextDirection = Vector2.left;
//             if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) nextDirection = Vector2.up;
//             if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) nextDirection = Vector2.down;

//             if (nextDirection.x != 0 && x == 0) nextDirection = new Vector2(0, y);
//             if (nextDirection.y != 0 && y == 0) nextDirection = new Vector2(x, 0);
//         }
//     }

//     private bool CanMove(Vector2 direction)
//     {
//         Vector2 origin = transform.position;
//         Vector2 destination = origin + direction;

//         playerCollider.enabled = false;
//         RaycastHit2D hit = Physics2D.Linecast(origin, destination);
//         playerCollider.enabled = true;

//         return hit.transform == null || hit.collider.isTrigger;
//     }

//     private void MoveTo(Vector2 direction)
//     {
//         targetPosition = (Vector2)transform.position + direction;
//         isMoving = true;
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Star"))
//         {
//             StoryPoint story = other.GetComponent<StoryPoint>();
            
//             if (story != null)
//             {
//                 uiTitle.text = story.title;
//                 uiDescription.text = story.description;
//             }

//             popupPanel.SetActive(true);
//             Time.timeScale = 0f;
//             other.GetComponent<StoryPoint>().enabled = false;
//         }
//     }

//     public void ClosePopup()
//     {
//         popupPanel.SetActive(false);
//         Time.timeScale = 1f;
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; // חובה כדי להשתמש ב-IEnumerator והשהיות זמנים

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private Collider2D playerCollider; 
    private SpriteRenderer spriteRenderer; // כדי שנוכל להעלים את החללית ויזואלית

    private Vector2 currentDirection = Vector2.zero;
    private Vector2 nextDirection = Vector2.zero;
    private Vector2 startPosition;
    
    // משתנה שמונע מהשחקן לזוז בזמן שהוא מחכה לחזור לחיים
    private bool isRespawning = false; 

    [Header("UI Elements (Drag from Canvas)")]
    public GameObject popupPanel;
    public TextMeshProUGUI uiTitle; 
    public TextMeshProUGUI uiDescription; 
    private GameObject lastVisitedStar = null;

    [Header("Audio & Effects")]
    public AudioClip hitSound;
    private AudioSource audioSource;
    public GameObject explosionPrefab; // התיבה שאליה נגרור את תבנית הפיצוץ

    void Start()
    {
        startPosition = transform.position;
        targetPosition = transform.position;
        
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // שואב את רכיב התמונה של החללית
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // void Update()
    // {
    //     // אם המשחק מושהה (פופאפ) או שאנחנו מתים כרגע - אל תאפשר תנועה
    //     if (Time.timeScale == 0f || isRespawning) return;

    //     HandleInput();

    //     if (!isMoving)
    //     {
    //         if (nextDirection != Vector2.zero)
    //         {
    //             if (CanMove(nextDirection))
    //             {
    //                 currentDirection = nextDirection;
    //                 MoveTo(currentDirection);
    //             }
    //             else if (currentDirection != Vector2.zero && CanMove(currentDirection))
    //             {
    //                 MoveTo(currentDirection);
    //             }
    //         }
    //     }
    //     else
    //     {
    //         transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

    //         if ((Vector2)transform.position == targetPosition)
    //         {
    //             isMoving = false;
    //         }
    //     }
    // }

    void Update()
{
    // 1. אם הפאנל פתוח (הזמן עומד)
    if (Time.timeScale == 0f)
    {
        // אנחנו מאזינים למקש רווח, ואם הוא נלחץ - סוגרים את הפאנל
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClosePopup();
        }
        
        // עוצרים כאן כדי שהחללית לא תנסה לטוס בזמן שהפאנל פתוח
        return; 
    }

    // 2. אם אנחנו בתהליך של התרסקות וחזרה לחיים - אל תאפשר תנועה
    if (isRespawning) return;

    // --- מכאן והלאה זה קוד התנועה הרגיל ---
    HandleInput();

    if (!isMoving)
    {
        if (nextDirection != Vector2.zero)
        {
            if (CanMove(nextDirection))
            {
                currentDirection = nextDirection;
                MoveTo(currentDirection);
            }
            else if (currentDirection != Vector2.zero && CanMove(currentDirection))
            {
                MoveTo(currentDirection);
            }
        }
    }
    else
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if ((Vector2)transform.position == targetPosition)
        {
            isMoving = false;
        }
    }
}

    private void HandleInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x == 0 && y == 0)
        {
            nextDirection = Vector2.zero;
            currentDirection = Vector2.zero;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) nextDirection = Vector2.right;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) nextDirection = Vector2.left;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) nextDirection = Vector2.up;
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) nextDirection = Vector2.down;

            if (nextDirection.x != 0 && x == 0) nextDirection = new Vector2(0, y);
            if (nextDirection.y != 0 && y == 0) nextDirection = new Vector2(x, 0);
        }
    }

    private bool CanMove(Vector2 direction)
    {
        Vector2 origin = transform.position;
        Vector2 destination = origin + direction;

        RaycastHit2D[] hits = Physics2D.LinecastAll(origin, destination);
        
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != playerCollider && !hit.collider.isTrigger)
            {
                return false; 
            }
        }
        return true; 
    }

    private void MoveTo(Vector2 direction)
    {
        targetPosition = (Vector2)transform.position + direction;
        isMoving = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Star"))
        {
            if (other.gameObject == lastVisitedStar) return;
            lastVisitedStar = other.gameObject;

            StoryPoint story = other.GetComponent<StoryPoint>();
            
            if (story != null)
            {
                uiTitle.text = story.title;
                uiDescription.text = story.description;
            }

            popupPanel.SetActive(true);
            Time.timeScale = 0f; 
        }
        // אם פגענו באסטרואיד ואנחנו לא כבר בתהליך חזרה לחיים
        else if (other.CompareTag("Hazard") && !isRespawning)
        {
            // משמידים את האסטרואיד שפגע בנו
            Destroy(other.gameObject);
            
            // מתחילים את תהליך ההשהיה והפיצוץ
            StartCoroutine(RespawnSequence());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Star") && other.gameObject == lastVisitedStar)
        {
            lastVisitedStar = null;
        }
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // --- הלב של התהליך: קורוטינה שמנהלת את הזמן ---
    private IEnumerator RespawnSequence()
    {
        isRespawning = true; // חוסם את ה-Update מלקבל פקודות תנועה
        
        // עוצרים את התנועה הנוכחית
        isMoving = false;
        nextDirection = Vector2.zero;
        currentDirection = Vector2.zero;

        // מעלימים את החללית (גם את התמונה וגם מכבים התנגשויות נוספות)
        spriteRenderer.enabled = false;
        playerCollider.enabled = false;

        // סאונד פגיעה
        if (hitSound != null) audioSource.PlayOneShot(hitSound);

        // יוצרים את הפיצוץ במיקום הנוכחי שלנו
        if (explosionPrefab != null)
        {
            GameObject exp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(exp, 1f); // משמיד את תמונת הפיצוץ אחרי שנייה אחת כדי שלא יישאר לנצח
        }

        // ממתינים 1.5 שניות (בזמן הזה המשחק ממשיך אבל אנחנו מוסתרים)
        yield return new WaitForSeconds(1.5f);

        // מחזירים את החללית להתחלה
        transform.position = startPosition;
        targetPosition = startPosition;

        // מחזירים את החללית להיראות ולפעול
        spriteRenderer.enabled = true;
        playerCollider.enabled = true;

        // משחררים את הנעילה כדי שנוכל לזוז שוב
        isRespawning = false;
    }
}