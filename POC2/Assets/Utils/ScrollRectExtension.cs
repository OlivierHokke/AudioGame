using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollRectExtension : MonoBehaviour {

    private RectTransform mine;
    private RectTransform scrollable;
    public float scrollSpeed = 5f;
    public float deceleration = 16f;
    private Vector2 toAdd;
    private Vector2 pixelBuffer;

	// Use this for initialization
	void Start () {
        mine = GetComponent<RectTransform>();
        scrollable = GetComponent<ScrollRect>().content;
        toAdd = Vector2.zero;
        pixelBuffer = Vector2.zero;
	}

	// Update is called once per frame
	void Update () 
    {
        float nowY = scrollable.anchoredPosition.y;
        toAdd += Vector2.up * 100f * scrollSpeed * -Input.GetAxis("Mouse ScrollWheel");
        toAdd.y = Mathf.Clamp(toAdd.y, 0f - nowY, scrollable.rect.height - mine.rect.height - nowY);
        Vector2 addNow = toAdd * Timeg.clamp01(deceleration);
        pixelBuffer += addNow;
        toAdd -= addNow;

        float moveX = Mathf.Round(pixelBuffer.x);
        float moveY = Mathf.Round(pixelBuffer.y);
        pixelBuffer.x -= moveX;
        pixelBuffer.y -= moveY;
        scrollable.anchoredPosition += new Vector2(moveX, moveY);
	}
}
