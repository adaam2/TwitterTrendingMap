using System.Collections.Generic;

namespace FinalUniProject.helperClasses
{
    // class needs to be static so that the same instance is added to and removed from at all times
    public static class ClientsConnected
    {
        // hashset containing all of the ids of connected clients
        public static HashSet<string> ConnectedClients = new HashSet<string>();

        public static int getNumberOfClients()
        {
            return ConnectedClients.Count;
        }
        public static void clearAllClients()
        {
            ConnectedClients.Clear();
        }
    }
}