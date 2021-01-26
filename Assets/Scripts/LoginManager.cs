using System;
using DarkRift;
using DarkRift.Client;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private GameObject loginWindow;
    [SerializeField] private InputField nameInput;
    [SerializeField] private Button submitLoginButton;

    void Start()
    {
        ConnectionManager.Instance.OnConnected += StartLoginProcess;
        submitLoginButton.onClick.AddListener(OnSubmitLogin);
        ConnectionManager.Instance.Client.MessageReceived += OnMessage;

        loginWindow.SetActive(false);
    }

    void OnDestroy()
    {
        ConnectionManager.Instance.OnConnected -= StartLoginProcess;
        ConnectionManager.Instance.Client.MessageReceived -= OnMessage;
    }

    public void StartLoginProcess()
    {
        loginWindow.SetActive(true);
    }

    private void OnMessage(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        {
            Debug.Log($"message: {message.Tag}");
            switch ((NetworkingData.Tags) message.Tag)
            {
                case NetworkingData.Tags.LoginRequestDenied:
                    OnLoginDecline();
                    break;
                case NetworkingData.Tags.LoginRequestAccepted:
                    //ConnectionManager.Instance.Client.MessageReceived += OnMessage;
                    OnLoginAccept(message.Deserialize<NetworkingData.LoginInfoData>());
                    break;
                default:
                    Debug.Log($"Unhandled tag in LoginManager.OnMessage: {message.Tag}");
                    break;
            }
        }
    }

    public void OnSubmitLogin()
    {
        Debug.Log("Login submitted.");
        
        if (!String.IsNullOrEmpty(nameInput.text))
        {
            loginWindow.SetActive(false);
            
            using (Message message = Message.Create((ushort)NetworkingData.Tags.LoginRequest, new NetworkingData.LoginRequestData(nameInput.text)))
            {
                ConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
                Debug.Log("Login message sent.");
            }
        }
    }

    private void OnLoginDecline()
    {
        //TODO show a "login failed" message
        loginWindow.SetActive(true);
    }

    private void OnLoginAccept(NetworkingData.LoginInfoData data)
    {
        ConnectionManager.Instance.Client.MessageReceived -= OnMessage;
        Debug.Log($"Login success, clientId = {data.Id}");
        
        ConnectionManager.Instance.PlayerId = data.Id;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
