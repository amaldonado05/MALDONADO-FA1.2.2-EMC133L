using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.8f; 
    public float jumpForce = 7f;

    [Header("Crouch Settings")]
    public float crouchScale = 0.6f;   
    public float crouchSpeed = 10f;    

    [Header("Ground Check Settings")]
    public Transform groundCheck;      
    public float groundDistance = 0.3f; 
    public LayerMask groundMask;       

    private Rigidbody rb;
    private bool isGrounded = true;
    private Vector3 defaultScale;
    private Vector3 crouchTargetScale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultScale = transform.localScale;
        crouchTargetScale = new Vector3(defaultScale.x, defaultScale.y * crouchScale, defaultScale.z);

        rb.freezeRotation = true;
        rb.angularDamping = 5f;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float h = 0f, v = 0f;
        if (Input.GetKey(KeyCode.A)) h = -1f;
        if (Input.GetKey(KeyCode.D)) h = 1f;
        if (Input.GetKey(KeyCode.W)) v = 1f;
        if (Input.GetKey(KeyCode.S)) v = -1f;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed *= sprintMultiplier; 

        Transform cam = Camera.main.transform;
        Vector3 forward = cam.forward; forward.y = 0f; forward.Normalize();
        Vector3 right = cam.right; right.y = 0f; right.Normalize();

        Vector3 desiredMove = (forward * v + right * h).normalized * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + desiredMove);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, crouchTargetScale, Time.deltaTime * crouchSpeed);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, defaultScale, Time.deltaTime * crouchSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
