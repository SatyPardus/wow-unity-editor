using UnityEngine;
using WebSocketSharp;

public class ServerSocket
{
    public bool IsConnected => webSocket != null && webSocket.ReadyState == WebSocketState.Open;

    private WebSocket webSocket;

    public void Connect(string ip, ushort port, string token)
    {
        if(webSocket != null)
        {
            Debug.LogError($"Already got a websocket instance");
            return;
        }

        webSocket = new WebSocket($"ws://{ip}:{port}/?token={token}");
        webSocket.OnOpen += WebSocket_OnOpen;
        webSocket.OnClose += WebSocket_OnClose;
        webSocket.OnError += WebSocket_OnError;
        webSocket.OnMessage += WebSocket_OnMessage;

        webSocket.Connect();
    }

    private void WebSocket_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log($"WebSocket_OnMessage {e.Opcode} {e.Data} {e.IsText} {e.IsBinary} {e.IsPing}");
    }

    private void WebSocket_OnError(object sender, ErrorEventArgs e)
    {
        Debug.Log($"WebSocket_OnError {e.Message} {e.Exception}");
    }

    private void WebSocket_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log($"WebSocket_OnClose {e.Code} {e.Reason}");
    }

    private void WebSocket_OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log($"WebSocket_OnOpen");
    }
}
