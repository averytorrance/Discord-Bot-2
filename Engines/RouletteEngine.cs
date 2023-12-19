using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Engines
{
    public class RouletteEngine
    {
        /// <summary>
        /// Dictionary mapping revolvers to a specific server
        /// </summary>
        private static Dictionary<ulong, Revolver> _revolvers = new Dictionary<ulong, Revolver>();
        
        /// <summary>
        /// Plays a round of Russian Roulette. Reloads the revolver in the event of a loss. 
        /// </summary>
        /// <param name="serverID">server ID to play the game for</param>
        /// <returns>true if the person is dead, false if the person lives</returns>
        public static string Play(ulong serverID)
        {
            Revolver currentRevolver = _getRevolver(serverID);
            bool isDead = currentRevolver.Fire();
            if (isDead)
            {
                Reload(serverID);
                return "You're dead.";
            }
            return $"click, current odds are {1} in {Revolver.GunCapacity - currentRevolver.TriggerPulls}";
        }

        /// <summary>
        /// Reloads the revolver for a specific server
        /// </summary>
        /// <param name="serverID">server ID</param>
        public static void Reload(ulong serverID)
        {
            _revolvers.Remove(serverID);
            _revolvers.Add(serverID, new Revolver());
        }

        /// <summary>
        /// Gets a revovler for a specific server
        /// </summary>
        /// <param name="serverID">server ID</param>
        /// <returns>the revolver for the input server</returns>
        private static Revolver _getRevolver(ulong serverID)
        {
            Revolver revolver;
            if (!_revolvers.ContainsKey(serverID))
            {
                _revolvers.Add(serverID, new Revolver());
            }
            _revolvers.TryGetValue(serverID, out revolver);
            return revolver;
        }

    }

    public class Revolver
    {
        /// <summary>
        /// Max Capacity for a revolver
        /// </summary>
        public static readonly int GunCapacity = 6;

        /// <summary>
        /// The maximum amount of rounds that can be loaded into the revolvers cylinder
        /// </summary>
        private const int _maxShots = 1;

        /// <summary>
        /// The maximum number of rotations when spinning the revolver cylinder
        /// </summary>
        private const int _maxSpins = 6*10;

        /// <summary>
        /// The total number of trigger pulls on this revolver
        /// </summary>
        public int TriggerPulls { get; private set; } = 0;

        /// <summary>
        /// Queue of shots
        /// </summary>
        private Queue<bool> _cylinder;

        /// <summary>
        /// Constructor
        /// </summary>
        public Revolver()
        {
            _cylinder = new Queue<bool>();

            _cylinder.Enqueue(true);
            for(int i = 0; i < GunCapacity-1; i++)
            {
                _cylinder.Enqueue(false);
            }
            Spin();
            _validateRevolver();
        }

        /// <summary>
        /// Shifts the cylinder queue a random number of times
        /// </summary>
        /// <returns></returns>
        public int Spin()
        {
            int totalSpins = (new Random(_maxSpins)).Next();
            for(int i=0; i < totalSpins; i++)
            {
                _cylinder.Enqueue(_cylinder.Dequeue());
            }
            _validateRevolver();
            return totalSpins;
        }

        /// <summary>
        /// Fires the revolver
        /// </summary>
        /// <returns>true if a round was fired, false if blank.</returns>
        public bool Fire()
        {
            /// We enqueue a false to the beginning of a queue because thjere is always going to be 6 shots in the revolver. 
            _cylinder.Enqueue(false);
            bool result = _cylinder.Dequeue();
            TriggerPulls++;
            _validateRevolver();
            return result;
        }

        /// <summary>
        /// Validates the revolver. Throws an exception if it fails
        /// </summary>
        /// <returns>null</returns>
        private void _validateRevolver()
        {
            if(_cylinder.Count != GunCapacity)
            {
                throw new Exception("Invalid revolver state.");
            }
        }
    }
}
