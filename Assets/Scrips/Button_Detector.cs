using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button_Detector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonPressed;
    [SerializeField] private Button slide_Button;
    [SerializeField] private PlayerMovement playerMovement;
    public void OnPointerDown(PointerEventData eventData){
        buttonPressed = true;
        // Check the X coordinate of the pointer event data
        float xValue = eventData.position.x;
        // Activate touchJump if X > 0, otherwise activate touchSlide
        if (xValue > 630)
        {
            playerMovement.touchJump = true;
            //StartCoroutine(DisableTouchJumpAfterDelay());
        }
        else
        {
            
            playerMovement.touchSlide = true;
        }
    }   

    public void OnPointerUp(PointerEventData eventData){
        buttonPressed = false;

        playerMovement.touchJump = false;
    
        playerMovement.touchSlide = false;
        
    }
    IEnumerator DisableTouchJumpAfterDelay()
    {
        yield return new WaitForSeconds(0.3f); // Wait for 0.3 seconds
        if (playerMovement.touchJump)
        {
            playerMovement.touchJump = false;
        }
    }
}
