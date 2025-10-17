using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleImageSwitcher : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Image targetImage;

    [Header("Sprites")]
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    [Header("Optionnel")]
    [SerializeField] private Color onColor = Color.white;
    [SerializeField] private Color offColor = Color.gray;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        toggle.onValueChanged.AddListener(OnToggleChanged);
        
        UpdateVisual(toggle.isOn);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        UpdateVisual(isOn);
    }

    private void UpdateVisual(bool isOn)
    {
        if (targetImage == null) return;
        
        if (onSprite && offSprite)
            targetImage.sprite = isOn ? onSprite : offSprite;

        targetImage.color = isOn ? onColor : offColor;
    }
}