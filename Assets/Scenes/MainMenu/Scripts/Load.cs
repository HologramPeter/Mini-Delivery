using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Load : MonoBehaviour
{

    GameState gameState;
    //public static void SavePlayer(GameState GameState)
    //{
    //    BinaryFormatter formatter = new BinaryFormatter();
    //    string Path = Application.persistentDataPath + "savelot_1";
    //    FileStream stream = new File(Path, FileMode.Create);
    //    GameState GameState = new GameState(GameState);
    //    formatter.Serialize(stream, GameState);
    //    stream.Close;
    //}

    public void LoadPlayer1()
    {

        string Path = Application.persistentDataPath + "/savelot_1";

        if (File.Exists(Path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(Path, FileMode.Open);
            GameState GameState = formatter.Deserialize(stream) as GameState;
            stream.Close();

            gameState = GameState;
            Debug.Log("GameState_1");

        }
        else
        {
            gameState = null;
            Debug.Log("Null");
        }
    }

    public void LoadPlayer2()
    {

        string Path = Application.persistentDataPath + "/savelot_2";

        if (File.Exists(Path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(Path, FileMode.Open);
            GameState GameState = formatter.Deserialize(stream) as GameState;
            stream.Close();

            gameState = GameState;
            Debug.Log("GameState_2");

        }
        else
        {
            gameState = null;
            Debug.Log("null");
        }
    }

    public void LoadPlayer3()
    {

        string Path = Application.persistentDataPath + "/savelot_3";

        if (File.Exists(Path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(Path, FileMode.Open);
            GameState GameState = formatter.Deserialize(stream) as GameState;
            stream.Close();

            gameState = GameState;
            Debug.Log("GameState_3");

        }
        else
        {
            gameState = null;
            Debug.Log("null");
        }
    }
}
