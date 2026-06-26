using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Checkpoint.Models
{
    public class CheckPointMetaData
    {
        public Guid Uuid { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Location { get; set; }

        public String SceneName { get; set; }

        public string SceneVersion { get; set; }

        public List<string> RegisteredObjects { get; set; } = new List<string>();

        public CheckPointMetaData()
        {
            Uuid = Guid.NewGuid();
            TimeStamp = DateTime.UtcNow;
        }
    }
}
