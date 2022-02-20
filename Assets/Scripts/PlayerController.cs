using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GravityBody))]
public class PlayerController : MonoBehaviour
{
    public float mouseSensitivityX = 1;
    public float moveSpeed = 15;
    private Vector3 moveDir;
    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Look rotation:
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);

        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if (other.CompareTag("Water"))
        {
            animator.SetBool("Swimming", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        if (other.CompareTag("Water"))
        {
            animator.SetBool("Swimming", false);
        }
    }
}
