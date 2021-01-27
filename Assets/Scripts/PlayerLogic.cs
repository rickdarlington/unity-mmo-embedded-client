using System.Linq;
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
        if (input.Keyinputs.Contains(true))
        {

            bool w = input.Keyinputs[0];
            bool a = input.Keyinputs[1];
            bool s = input.Keyinputs[2];
            bool d = input.Keyinputs[3];
            bool space = input.Keyinputs[4];

            moveDirection = new Vector2(0, 0);

            if (w) moveDirection.y = 1;
            if (s) moveDirection.y = -1;
            
            if (a) moveDirection.x = -1;
            if (d) moveDirection.x = 1;

            moveDirection = moveDirection * Time.fixedDeltaTime;
            Debug.Log($"movedirection {moveDirection.x}, {moveDirection.y}");

            if (CharacterController.enabled && input.Keyinputs.Contains(true))
            {
                CharacterController.Move(new Vector3(moveDirection.x, moveDirection.y, 0));
            }
        }

        return new NetworkingData.PlayerStateData(currentStateData.Id, new System.Numerics.Vector2(transform.localPosition.x, transform.localPosition.y), 0);
    }
}