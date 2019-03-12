using UnityEngine;
using System.Net;
using System.IO;
using System.Text;
using System;

public class Networking : MonoBehaviour
{
    //premjestiti u neke globalne varijable
    public string endpoint = "http://192.168.0.18:8080/api/sglab/machines/";
    private static readonly string USERNAME = "sglabadmin";
    private static readonly string PASSWORD = "sglabadmin";

    float time = 5.0f;

    string machineId = "1";

    [Serializable]
    public class Machine
    {
        public int id;
        public string name;
        public string description;
        public double activePower;
        public double reactivePower;
        public double apparentPower;

        public override string ToString()
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

    [Serializable]
    public class Command
    {
        public string value;
        public object valueObj;

    }

    void Start()
    {
        EstablishCommunication(machineId);

        Command command = new Command();
        command.value = "value";
        command.valueObj = true;
        ExecuteCommand(machineId, "stop", JsonUtility.ToJson(command));
    }

    void EstablishCommunication(string id)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(USERNAME + ":" + PASSWORD);
        string encoded = Convert.ToBase64String(plainTextBytes);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint + id);

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

        Machine machine = ParseJson(json.ToString());
        Debug.Log(machine.ToString());

        responseStream.Close();
        myWebResponse.Close();
    }

    public Machine ParseJson(string json)
    {
        Machine machine = JsonUtility.FromJson<Machine>(json);

        return machine;
    }

    // Update is called once per frame
    void UpdateTimer()
    {

        time -= Time.deltaTime;
        int seconds = (int)(time % 60);

        if (seconds == 0)
        {
            EstablishCommunication(machineId);
            
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

        EstablishCommunication(machineId);
        StartCoundownTimer();
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("Komunikacija sa serverom");
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Zatvaranje menija");

        CancelInvoke();
    }

    //commands = {pref, start, stop}
    void ExecuteCommand(string id, string command, string json)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(USERNAME + ":" + PASSWORD);
        string encoded = Convert.ToBase64String(plainTextBytes);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint + id + "/" + command);

        request.Headers.Add("Authorization", "Basic " + encoded);
        request.ContentType = "application/json";
        request.Method = "POST";

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
        }

        var httpResponse = (HttpWebResponse)request.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
            Debug.Log(result);
        }
    }

}
