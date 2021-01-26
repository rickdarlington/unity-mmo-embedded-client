using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DarkRift;
using UnityEngine;

[RequireComponent(typeof(PlayerLogic))]
[RequireComponent(typeof(PlayerInterpolation))]
public class ClientPlayer : MonoBehaviour
{
    private PlayerLogic playerLogic;

    private ushort id;
    private string playerName;
    private bool isOwn;

    private int health;
    
    private float moveSpeed = 40f;
    private Vector2 moveDirection;

    private PlayerInterpolation Interpolation;

    [Header("Settings")]
    [SerializeField]
    private float sensitivityX;
    [SerializeField]
    private float sensitivityY;
    
    void Awake()
    {
        playerLogic = GetComponent<PlayerLogic>();
        Interpolation = GetComponent<PlayerInterpolation>();
    }
    
    public void Initialize(ushort id, string playerName)
    {
        this.id = id;
        this.playerName = playerName;
        //NameText.text = this.playerName;
        if (ConnectionManager.Instance.PlayerId == id)
        {
            isOwn = true;
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0,0,0);
            Camera.main.transform.localRotation = Quaternion.identity;
            Interpolation.CurrentData = new NetworkingData.PlayerStateData(id, new System.Numerics.Vector2(0,0), 0 );
        }
    }
    
    public void OnServerDataUpdate(NetworkingData.PlayerStateData data)
    {
        if (isOwn)
        {
        
        }
        else
        {
            Interpolation.SetFramePosition(data);
        }
    }

    void FixedUpdate()
    {
        if (isOwn)
        {
            bool[] inputs = new bool[6];
            inputs[0] = Input.GetKey(KeyCode.W);
            inputs[1] = Input.GetKey(KeyCode.A);
            inputs[2] = Input.GetKey(KeyCode.S);
            inputs[3] = Input.GetKey(KeyCode.D);
            inputs[4] = Input.GetKey(KeyCode.Space);

            if (inputs.Contains(true))
            {
                NetworkingData.PlayerInputData inputData = new NetworkingData.PlayerInputData(inputs, 0, 0);

                transform.position =
                    new Vector3(Interpolation.CurrentData.Position.X, Interpolation.CurrentData.Position.Y, 0);
                NetworkingData.PlayerStateData nextStateData =
                    playerLogic.GetNextFrameData(inputData, Interpolation.CurrentData);
                Interpolation.SetFramePosition(nextStateData);

                using (Message message = Message.Create((ushort) NetworkingData.Tags.GamePlayerInput, inputData))
                {
                    ConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
                }
            }
        }
    }
}