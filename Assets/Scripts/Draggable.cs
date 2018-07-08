using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour {

    public Canvas displayCanvas;
    private Camera uiCamera;

    private bool beginMovement = false;

    //Setting up an event listener when the object dragging is over
    public delegate void OnDragOver(Transform sourceObject );
    public static event OnDragOver onDragOver;


    // Monobehaviour Lifecycle Hook
    void Start ()
    {
        this.displayCanvas = Camera.main.transform.Find("UI").GetComponent<Canvas>();
        this.uiCamera = this.transform.root.GetComponent<Camera>();
    }

    private void Update()
    {
        if(Input.GetMouseButton(0) && this.CheckIfRayHits(this.gameObject) && !this.beginMovement)
        {
            this.beginMovement = true;
        }   
        if (Input.GetMouseButton(0) && this.beginMovement)
        {   
            //Finding out the cordinates from the world space to the canvas space
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.displayCanvas.transform as RectTransform, Input.mousePosition, this.displayCanvas.worldCamera, out mousePosition);
            this.transform.position = this.displayCanvas.transform.TransformPoint(mousePosition);
        }
        if (Input.GetMouseButtonUp(0) && this.beginMovement)
        {
            if(onDragOver != null)
            {
                onDragOver(this.transform);
            }
        }
    }
    
    /// <summary>
    /// Simply Check if 2D Raycast is successful and ray hits the supplied gameobject. In this case raycast is straight from the camera to the mouse position. Make sure the gameobject has a 2DCollider Component attached
    /// </summary>
    /// <param name="objectToCheck">GameObject to check the raycast with</param>
    /// <returns></returns>
    private bool CheckIfRayHits(GameObject objectToCheck)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(this.uiCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 600);
        return rayHit ? rayHit.collider.gameObject == objectToCheck : false;
    }
}
