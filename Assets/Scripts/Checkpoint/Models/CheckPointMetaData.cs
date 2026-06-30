using System;

namespace Assets.Scripts.Checkpoint.Models
{
    public class CheckPointMetaData
    {
        public Guid Uuid { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Location { get; set; }

        public String SceneName { get; set; }

        public string SceneVersion { get; set; }

        public ObjectDataWrapperClass ObjectDataWrapper { get; set; }

        public CheckPointMetaData()
        {
            Uuid = Guid.NewGuid();
            TimeStamp = DateTime.UtcNow;
        }

        public CheckPointMetaData(Guid uid, string sceneName, string sceneVersion, string location)
        {
            Uuid = Guid.NewGuid();
            Location = location;
            SceneName = sceneName;
            SceneVersion = sceneVersion;
            TimeStamp = DateTime.UtcNow;
        }
    }
}
