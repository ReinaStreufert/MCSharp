using MCSharp.GameplayComponents.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSharp.GameplayComponents
{
    public class Player
    {
        public Client AttachedClient;
        public PlayerEntity Entity;
        public Zone CurrentZone;
    }
}
