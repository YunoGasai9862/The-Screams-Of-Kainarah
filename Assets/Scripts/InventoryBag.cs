using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryBag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private const float HIGHLIGHT_COLOR_R = 166;
    private const float HIGHLIGHT_COLOR_G = 161;
    private const float HIGHLIGHT_COLOR_B = 161;

    [SerializeField] InventoryPouchClickEvent inventoryPouchClickEvent;
    [SerializeField] InventoryPouchPanelEvent inventoryPouchPanelClickEvent;
    [SerializeField] Image uiBagImage;

    private Color m_originalColor;

    private bool IsPouchPanelActive { get; set; }

    private void Awake()
    {
        m_originalColor = uiBagImage.color;
    }

    private void Start()
    {
        inventoryPouchPanelClickEvent.AddListener(IsPanelActive);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if its true (panel active), send false (to close it), if false (closed), send true (open it)
        inventoryPouchClickEvent.Invoke(IsPouchPanelActive? false: true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        uiBagImage.color = new Color(HIGHLIGHT_COLOR_R/255f, HIGHLIGHT_COLOR_G/255f, HIGHLIGHT_COLOR_B/255f, 1.0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiBagImage.color = m_originalColor;
    }

    public void IsPanelActive(bool isPanelActive)
    {
        IsPouchPanelActive = isPanelActive;
    }
}
