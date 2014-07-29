using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPartyServer
{
    class GameTime
    {
        public TimeSpan Elapsed;

        DateTime previousTime = DateTime.UtcNow;

        public void Update()
        {
            Elapsed = DateTime.UtcNow - previousTime;
            previousTime = DateTime.UtcNow;
        }
    }
}