using UnityEngine;

public class InfiniteBackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.2f;
    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
        offset = rend.material.mainTextureOffset;
    }

    void Update()
    {
        offset.y -= scrollSpeed * Time.deltaTime;
        rend.material.mainTextureOffset = offset;
    }
}