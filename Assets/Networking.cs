using UnityEngine;
using System.Net;
using System.IO;
using System.Text;
using System;
using System.Web.Script.Serialization;

public class Networking : MonoBehaviour
{
    //premjestiti u neke globalne varijable
    public string endpoint = "http://10.129.0.214:8080/api/sglab/machines/1";
    private static readonly string USERNAME = "sglabadmin";
    private static readonly string PASSWORD = "sglabadmin";

    float time = 5.0f;

    public class Machine
    {
        public static readonly string ID = "id";
        public static readonly string NAME = "name";
        public static readonly string DESCRIPTION = "description";
        public static readonly string ACTIVE_POWER = "activePower";
        public static readonly string REACTIVE_POWER = "reactivePower";
        public static readonly string APPARENT_POWER = "apparentPower";

        private int id { get; set; }
        private string name { get; set; }
        private string description { get; set; }
        private double activePower { get; set; }
        private double reactivePower { get; set; }
        private double apparentPower { get; set; }
        
        public string ToString()
        {
            return "Machine{" +
                    "id=" + id +
                    ", name='" + name + '\'' +
                    ", description='" + description + '\'' +
                    ", activePower=" + activePower +
                    ", reactivePower=" + reactivePower +
                    ", apparentPower=" + apparentPower +
                    '}';
        }
    }

    void Start()
    {

        EstablishCommunication();
        StartCoundownTimer();
    }

    void EstablishCommunication()
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(USERNAME + ":" + PASSWORD);
        string encoded = Convert.ToBase64String(plainTextBytes);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);

        request.Headers.Add("Authorization", "Basic " + encoded);
        request.ContentType = "application/json";
        request.Method = "GET";

        var myWebResponse = request.GetResponse();
        var responseStream = myWebResponse.GetResponseStream();
        if (responseStream == null)
        {
            //nes smisleno ako dode do greske
            Debug.Log("Error! Response Stream is Null...");
            // throw new System.Exception();
        }

        var myStreamReader = new StreamReader(responseStream, Encoding.Default);
        var json = myStreamReader.ReadToEnd();

        //add pasre JSON
        Debug.Log(json.ToString());
        Machine machine = ParseJson(json.ToString());

        responseStream.Close();
        myWebResponse.Close();
    }

    public Machine ParseJson(string json)
    {
        Machine machine = new Machine();

        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
        machine = jsonSerializer.Deserialize<Machine>(json);

        return machine;
    }

    // Update is called once per frame
    void UpdateTimer()
    {

        time -= Time.deltaTime;
        int seconds = (int)(time % 60);

        if (seconds == 0)
        {
            EstablishCommunication();
            
            time = 6;
        }
    }

    void StartCoundownTimer()
    {
        time = 5;
        InvokeRepeating("UpdateTimer", 0.0f, 0.01667f);
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Otvaranje menija");
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Zatvaranje menija");
    }
}
