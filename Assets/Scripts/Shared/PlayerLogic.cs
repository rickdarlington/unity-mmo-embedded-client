using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerLogic : MonoBehaviour
{
    private float moveSpeed = 40f;

    public CharacterController CharacterController { get; private set; }

    void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        CharacterController.enabled = true;
    }

    public NetworkingData.PlayerStateData GetNextFrameData(ushort id, NetworkingData.PlayerInputData input)
    {
        Vector2 moveDirection = new Vector2(0, 0);
            
        bool w = input.Keyinputs[0];
        bool a = input.Keyinputs[1];
        bool s = input.Keyinputs[2];
        bool d = input.Keyinputs[3];
        bool space = input.Keyinputs[4];
            
        if (w || a || s || d || space)
        {
            if (w) moveDirection.y = 1;
            if (s) moveDirection.y = -1;

            if (a) moveDirection.x = -1;
            if (d) moveDirection.x = 1;

            moveDirection = moveDirection * Time.fixedDeltaTime;
        }
        
        CharacterController.Move(moveDirection);

        return new NetworkingData.PlayerStateData(id, transform.position, 0);
    }
}