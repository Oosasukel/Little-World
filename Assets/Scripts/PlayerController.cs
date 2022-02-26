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

    [Header("Player Step Climb")]
    [SerializeField]
    GameObject stepRayUpper;
    [SerializeField]
    GameObject stepRayLower;
    [SerializeField]
    float stepHeight = 0.3f;
    [SerializeField]
    float stepSmooth = 0.1f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }

    void Update()
    {
        // Look rotation:
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);

        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetMouseButtonDown(0)) animator.SetTrigger("Attacking");
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);
        StepClimb();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            animator.SetBool("Swimming", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            animator.SetBool("Swimming", false);
        }
    }

    void StepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {

            RaycastHit hitUpper;
            if (Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.1f))
            {
                // @TODO substituir o up para ser relativo a rotação do personagem (e os ifs também)
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }

        // @TODO replicar para +45º e -45º
    }
}
