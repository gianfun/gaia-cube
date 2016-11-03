using UnityEngine;
using System.Collections;

public class OurWebSocket : MonoBehaviour {

    IEnumerator Start()
    {
        WebSocket w = new WebSocket(new System.Uri("ws://127.0.0.1:6437/v7.json"));
        yield return StartCoroutine(w.Connect());

        int i = 0;
        while (true)
        {
            string reply = w.RecvString();
            if (reply != null)
            {
                Debug.Log("Received: " + reply);
            }
            if (w.error != null)
            {
                Debug.LogError("Error: " + w.error);
                break;
            }
            yield return 0;
        }
        Debug.Log("Exit loop");
        w.Close();
    }
}
