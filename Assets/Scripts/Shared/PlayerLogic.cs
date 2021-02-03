using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerLogic : MonoBehaviour
{
    public CharacterController CharacterController { get; private set; }
    
    void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
    }

    public NetworkingData.PlayerStateData GetNextFrameData(ushort id, NetworkingData.PlayerInputData input)
    {
        bool w = input.Keyinputs[0];
        bool a = input.Keyinputs[1];
        bool s = input.Keyinputs[2];
        bool d = input.Keyinputs[3];
        bool space = input.Keyinputs[4];

        Vector2 movement = Vector2.zero;

        if (w)
        {
            movement += Vector2.up;
        }
        if (a)
        {
            movement += Vector2.left;
        }
        if (s)
        {
            movement += Vector2.down;
        }
        if (d)
        {
            movement += Vector2.right;
        }

        movement = movement * Time.fixedDeltaTime;


        CharacterController.Move(movement);

        return new NetworkingData.PlayerStateData(id, transform.localPosition, input.LookDirection);
    }
}