[System.Serializable]

public class SaveObject
{
    // Save file information
    public string name;
    public long timestamp;
    // Story progress (ink global variables)
    public string inkVariablesJson;
    // Level and player data
    public string currentScene;
    public SerializableVector3 playerPosition;
    public SerializableVector3 playerRotationEuler;
    public SerializableVector3 cameraRotationEuler;

}
