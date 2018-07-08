using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

#region Public_Variables

#endregion

#region Private_Variables
    private Animator animator;

    private Ray ray;
    private RaycastHit rayHit;

    private int animIndex = 0;
#endregion

    // Use this for initialization
    private void Awake () {
        this.animator = this.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	private void Update () {
        if (Input.GetMouseButtonDown(0) && this.CheckIfHitsMouse(this.gameObject, Input.mousePosition))
            this.SwapAnimation();
    }

    private bool CheckIfHitsMouse(GameObject objectToCheck , Vector3 position)
    {
        //Updating the Ray as per the mouse
        this.ray = Camera.main.ScreenPointToRay(position);
        return Physics.Raycast(this.ray, out this.rayHit, 500) ? this.rayHit.collider.gameObject == objectToCheck : false;
    }

    //Swap Animations
    private void SwapAnimation()
    {
        this.animator.SetInteger("animIndex", Random.Range(1, 6));
    }

    //For Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.ray.origin, this.ray.direction * 100);
    }

    public void ResetState()
    {
        this.animator.SetInteger("animIndex", 0);
    }
}
