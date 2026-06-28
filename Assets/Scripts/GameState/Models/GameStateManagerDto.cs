using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.GameState.Models
{
    public class GameStateManagerDto
    {
        public string Location { get; set; }

        public string JsonBlob { get; set; }

        public override string ToString()
        {
            return $"Location: {Location}, JsonBlob: {JsonBlob}";
        }
    }
}
