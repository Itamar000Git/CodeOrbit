using UnityEngine;

public class StoryPoint : MonoBehaviour
{
    public string title;
    [TextArea(3, 10)] // מגדיל את תיבת הטקסט ביוניטי כדי שיהיה נוח לכתוב
    public string description;
}