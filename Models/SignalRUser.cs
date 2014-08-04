using FinalUniProject.Models;

namespace FinalUniProject
{
    public class SignalRUser
    {
        public string ConnectionId { get; set; }
        public bool isStreamRunning = true;
        public BoundingBoxPoint userBoundingBox { get; set; }
    }
}