using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Transform target;
    public Image foregroundImage;
    public Image backgroundImage;
    // Update is called once per frame
    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            return;
        }
        
        Vector3 direction = (target.position - cam.transform.position).normalized;
        bool isBehind = Vector3.Dot(direction, cam.transform.forward) <= 0.0f;
        foregroundImage.enabled = !isBehind;
        backgroundImage.enabled = !isBehind;
        Vector3 screenPos = cam.WorldToScreenPoint(target.position + new Vector3(0f, 0.15f, 0f));
        transform.position = screenPos;
    }
    public void SetHealthBarPercentage(float percentage)
    {
        float parentWidth = GetComponent<RectTransform>().rect.width;
        float width = parentWidth * percentage;
        foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}
