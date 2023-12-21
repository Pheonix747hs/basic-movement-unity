using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sliding : MonoBehaviour
{
    public float slideDuration = 1.0f;
    public float slideSpeed = 10.0f;
    public float slideAngle = 45.0f;
    public KeyCode slideKey = KeyCode.Tab;

    private bool isSliding = false;
    private float slideTimer = 0.0f;
    private Vector3 slideDirection;


    private void Update()
    {
        if (Input.GetKeyDown(slideKey) && !isSliding)
        {
            // Start sliding
            isSliding = true;
            slideDirection = transform.forward;
            slideDirection.y = -Mathf.Tan(slideAngle * Mathf.Deg2Rad);
            slideDirection = slideDirection.normalized;
            slideTimer = slideDuration;
        }
        else if (isSliding)
        {
            // Continue sliding
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0.0f)
            {
                // End sliding
                isSliding = false;
            }
            else
            {
                transform.position += slideDirection * slideSpeed * Time.deltaTime;
            }
        }
    }
}
