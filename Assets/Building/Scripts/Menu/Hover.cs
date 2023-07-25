using UnityEngine;

public class MenuController : MonoBehaviour
{
    public float moveSpeed = 600;

    private Vector2 startPos;
    private Vector2 target;

    void Start()
    {
        var tr = transform as RectTransform;
        target = tr.anchoredPosition;
    }

    void Update()
    {
        var tr = transform as RectTransform;
        tr.anchoredPosition = Vector2.MoveTowards(tr.anchoredPosition, target, moveSpeed * Time.deltaTime);

        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            switch (touch.phase)
            {
                case TouchPhase.Began: startPos = touch.position; break;
                case TouchPhase.Moved:
                    //swipe horizontal?
                    if (touch.position.x - startPos.x > 20)
                        target = new Vector2(tr.sizeDelta.x / 2, tr.anchoredPosition.y);//show menu
                    if (touch.position.x - startPos.x < -20)
                        target = new Vector2(-tr.sizeDelta.x / 2, tr.anchoredPosition.y);//hide menu
                    break;
            }
        }
    }
}