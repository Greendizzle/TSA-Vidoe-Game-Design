using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 1f;
    public float RotationSpeed = 1f;

    CharacterController _controller;

	// Use this for initialization
	void Awake ()
    {
        _controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * MoveSpeed;
        moveDirection = transform.TransformDirection(moveDirection);

        transform.Rotate(0, Input.GetAxis("Mouse X") * RotationSpeed, 0);
        _controller.SimpleMove(moveDirection);       
    }
}
