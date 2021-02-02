using UnityEngine;

public class PlayerInterpolation : MonoBehaviour
{
    private float lastInputTime;

    public NetworkingData.PlayerStateData CurrentData { get; set; }
    public NetworkingData.PlayerStateData PreviousData { get; private set; }

    public void SetFramePosition(NetworkingData.PlayerStateData data)
    {
        RefreshToPosition(data, CurrentData);
    }

    public void RefreshToPosition(NetworkingData.PlayerStateData data, NetworkingData.PlayerStateData prevData)
    {
        PreviousData = prevData;
        CurrentData = data;
        lastInputTime = Time.fixedTime;
    }

    public void Update()
    {
        float timeSinceLastInput = Time.time - lastInputTime;
        float t = timeSinceLastInput / Time.fixedDeltaTime;
        
        transform.position = Vector2.LerpUnclamped(
            new Vector2(PreviousData.Position.x, PreviousData.Position.y), 
            new Vector2(CurrentData.Position.x, CurrentData.Position.y), 
            t);
    }
}