using System.Linq;
using Shared;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

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

    public NetworkingData.PlayerStateData GetNextFrameData(
        NetworkingData.PlayerInputData input,
        NetworkingData.PlayerStateData currentStateData)
    {
        Vector2 moveDirection = FrameData.GetNextFrameData(input, Time.deltaTime);
        
        if (CharacterController.enabled && input.Keyinputs.Contains(true))
        {
            CharacterController.Move(new Vector3(moveDirection.X, moveDirection.Y, 0));
        }
        return new NetworkingData.PlayerStateData(currentStateData.Id, new Vector2(transform.localPosition.x, transform.localPosition.y), 0);
    }
}