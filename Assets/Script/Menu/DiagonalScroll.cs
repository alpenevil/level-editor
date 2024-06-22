using UnityEngine;
using UnityEngine.UI;

public class DiagonalScroll : MonoBehaviour
{
    public float scrollSpeed = 0.1f; 
    private RawImage rawImage;
    private Vector2 offset;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        offset = Vector2.zero;
    }

    void Update()
    {
       
        float scrollAmount = scrollSpeed * Time.deltaTime;
        offset.x += scrollAmount;
        offset.y += scrollAmount;

      
        if (offset.x > 1.0f) offset.x -= 1.0f;
        if (offset.y > 1.0f) offset.y -= 1.0f;

        rawImage.uvRect = new Rect(offset, rawImage.uvRect.size);
    }
}
