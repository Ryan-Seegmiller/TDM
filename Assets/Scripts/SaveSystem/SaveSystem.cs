using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public struct GameData
{
    public int score;
    public float X;
    public float Y;

    public GameData(int s, Vector3 loc) 
    { 
        score = s;
        X = loc.x;
        Y = loc.y;
    }

}

[Serializable]
public struct DeathData
{
    public int playertype;
    public float X,Y;
    public DeathData(int i, Vector3 loc)
    {
        playertype = i;
        X = loc.x;
        Y = loc.y;
    }
}



public static class SaveSystem
{
    //varying this will create multiple save files
    static string filetype = ".gamesave";
    static string defaultFile => System.DateTime.Now.ToString("MMddyy");//temp

    static string logtype = ".deathlog";
    


    public static void SaveGame(GameData[] gameInfo) //should use a single data type holding everything
    {
        BinaryFormatter formater = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + defaultFile +filetype; //arbitrary filetype

        //Debug.Log("SaveFile created at " + path.ToString());
        //open the path to the file location
        FileStream stream = new FileStream(path, FileMode.Create);



        GameData[] data = gameInfo; //remeber the game data 


        formater.Serialize(stream, data);        //convert data to binary
        stream.Close();                         //close the path, if you open the stream be sure to close it!!!
        //Debug.Log("Save Completed at " + path.ToString());
    }

    public static void LogGameFail(DeathData deathInfo) //int as placeholder, should use data type holding everything
    {
        //gather existing data
        DeathData[] oldData = LoadDeathData();
        //Debug.Log(oldData == null);
        int length = (oldData==null)? 1 : oldData.Length + 1;


        DeathData[] newData = new DeathData[length];

        //populate new data container
        for (int i = 0; i < length; i++)
        {
            if (i < length -1)
            {
                newData[i] = oldData[i];
            }
            else
            {
                newData[i] = deathInfo;
            }
        }

        BinaryFormatter formater = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + defaultFile + logtype; //arbitrary filetype

        //Debug.Log("SaveFile created at " + path.ToString());
        //open the path to the file location
        FileStream stream = new FileStream(path, FileMode.Create);

        DeathData[] data = newData; //record the game data in the stream

        formater.Serialize(stream, data);        //convert data to binary
        stream.Close();                         //close the path, if you open the stream be sure to close it!!!
       // Debug.Log("Save Completed at " + path.ToString());
    }







    public static DeathData[] LoadDeathData()
    {
        string path = Application.persistentDataPath + "/" + defaultFile + logtype;
        //Debug.Log("Looking for SaveFile at " + path.ToString());
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            //open the path
            FileStream stream = new FileStream(path, FileMode.Open);


            DeathData[] data = formatter.Deserialize(stream) as DeathData[]; //convert the binary to a usable format

            stream.Close();                        //don't cross the streams, close it every time you open it!!!
            //Debug.Log("Log File loaded from " + path.ToString());

            return data;
        } else
        {
            Debug.Log(" Log file not found in " + path);
            return null;
        }
    }

    public static GameData[] LoadGameData()
    {
        string path = Application.persistentDataPath + "/" + defaultFile + filetype;
        //Debug.Log("Looking for SaveFile at " + path.ToString());
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            //open the path
            FileStream stream = new FileStream(path, FileMode.Open);


            GameData[] data = formatter.Deserialize(stream) as GameData[]; //convert the binary to a usable format

            stream.Close();                        //don't cross the streams, close it every time you open it!!!
            //Debug.Log("SaveFile loaded from " + path.ToString());

            return data;
        }
        else
        {
            Debug.LogError(" Save file not found in " + path);
            return null;
        }
    }
}
