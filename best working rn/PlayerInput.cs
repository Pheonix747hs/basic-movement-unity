using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public float movespeed = 5;
	public float speed;
	public float runspeed = 10;
    public float couchspeed = 3.5f;
	public float jumpHeight = 15;
	public PhysicalCC physicalCC;


	public Transform bodyRender;
	IEnumerator sitCort;
	public bool isSitting;
	public bool isSprinting;
	public KeyCode jumpkey=KeyCode.Space;
	public KeyCode runkey=KeyCode.Q;
    private Vector3 currpos;


    private void Start()
    {
        isSprinting = false;
		speed = movespeed;
    }
    void Update()
    {
        if (physicalCC.isGround)
        {
            physicalCC.moveInput = Vector3.ClampMagnitude(transform.forward
                            * Input.GetAxis("Vertical")
                            + transform.right
                            * Input.GetAxis("Horizontal"), 1f) * speed;

            if (Input.GetKey(jumpkey))
            {
                physicalCC.inertiaVelocity.y = 0f;
                physicalCC.inertiaVelocity.y += jumpHeight;
            }

            if (Input.GetKeyDown(KeyCode.C) && sitCort == null && !isSprinting)
            {
                speed = movespeed;
                sitCort = sitDown();
                StartCoroutine(sitCort);
            }

            if (Input.GetKeyDown(runkey) && sitCort == null && !isSitting)
            {
                isSprinting=!isSprinting;
                speed = isSprinting ? runspeed : movespeed  ;
            }


            // Add slide functionality here
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                StartCoroutine(Slide());
            }
        }
    }

    IEnumerator sitDown()
	{
		if (isSitting && Physics.Raycast(transform.position, Vector3.up, physicalCC.cc.height * 1.5f))
		{
			sitCort = null;
			yield break;
		}
		isSitting = !isSitting;

		float t = 0;
		float startSize = physicalCC.cc.height;
		float finalSize = isSitting ? physicalCC.cc.height / 2 : physicalCC.cc.height * 2;

		Vector3 startBodySize = bodyRender.localScale;
		Vector3 finalBodySize = isSitting ? bodyRender.localScale - Vector3.up * bodyRender.localScale.y / 2f : bodyRender.localScale + Vector3.up * bodyRender.localScale.y;

		

		speed = isSitting ? couchspeed : movespeed;

		jumpHeight = isSitting ? jumpHeight / 2 : jumpHeight * 2;
		
		while (t < 0.2f)
		{
			t += Time.deltaTime;
			physicalCC.cc.height = Mathf.Lerp(startSize, finalSize, t / 0.2f);
			bodyRender.localScale = Vector3.Lerp(startBodySize, finalBodySize, t / 0.2f);
			yield return null;
		}

		sitCort = null;
		yield break;
	}

    IEnumerator Slide()
    {
        // Reduce the player's height and speed during the slide
        float startHeight = physicalCC.cc.height;
        float slideHeight = startHeight / 2;
        Vector3 startBodySize = bodyRender.localScale;
        Vector3 finalBodySize =bodyRender.localScale - Vector3.up * bodyRender.localScale.y / 2f;
        speed /= 2f;
        float t = 0;

        while (t < 0.2f)
        {
            t += Time.deltaTime;
            physicalCC.cc.height = Mathf.Lerp(startHeight, slideHeight, t / 0.2f);
            bodyRender.localScale = Vector3.Lerp(startBodySize, finalBodySize, t / 0.2f);
        }
        // Apply a force in the direction the player is facing to initiate the slide
        physicalCC.inertiaVelocity += transform.forward * speed;
        t = 0;

        // Wait for a short amount of time before restoring the player's height and speed
        yield return new WaitForSeconds(0.5f);
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            physicalCC.cc.height = Mathf.Lerp(slideHeight, startHeight, t / 0.2f);
            bodyRender.localScale = Vector3.Lerp(finalBodySize, startBodySize, t / 0.2f);
        }

        speed *= 2f;
    }


}
