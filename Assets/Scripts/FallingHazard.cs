using UnityEngine;

public class FallingHazard : MonoBehaviour
{
    public float fallSpeed = 4f;

    void Update()
    {

        // הוספנו את המילה Space.World בסוף הפקודה!
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime, Space.World);

        // השמדת האסטרואיד כשהוא יוצא מהמסך כדי לחסוך בזיכרון
        if (transform.position.y < -12f)
        {
            Destroy(gameObject);
        }
    }
}