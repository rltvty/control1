using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using PLAN;

public class setlevel : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {

    }

    public PNumber test;

    public PRuntime runtime;

    public string homeid;


    public void SetValue(float value)
    {
        /*WWWForm form = new WWWForm();
        form.AddField("level", value.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/test", form))
        {
            www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }*/

        try
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://localhost:3000/test");

            var postData = "level=" + value.ToString();
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
        catch (WebException exp)
        {
            Debug.Log("Unabled to connect to server: " + exp.Message);
        }

        //Debug.Log("Response: " + responseString);
    }
}
