using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerLogic : MonoBehaviour
{
    private float moveSpeed = 40f;
    private Vector2 moveDirection;
    
    public CharacterController CharacterController { get; private set; }

    void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        CharacterController.enabled = true;
    }

    public NetworkingData.PlayerStateData GetNextFrameData(NetworkingData.PlayerInputData input,
        NetworkingData.PlayerStateData currentStateData)
    {

        moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveSpeed = Mathf.Clamp(moveDirection.magnitude, 0.0f, 1.0f);
        moveDirection.Normalize();

        Vector2 movement = Vector2.zero;
        movement = movement * moveSpeed;
        movement = movement * Time.fixedDeltaTime;

        CharacterController.Move(new Vector3(0, -0.001f, 0));
        if (CharacterController.enabled)
        {
            CharacterController.Move(new Vector3(movement.x, movement.y, 0));
        }

        return new NetworkingData.PlayerStateData(currentStateData.Id, new System.Numerics.Vector2(transform.localPosition.x, transform.localPosition.y), 0);
    }
}