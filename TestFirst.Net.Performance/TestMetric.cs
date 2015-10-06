namespace TestFirst.Net.Performance
{
    public class TestId
    {
        internal TestId(string threadId, string agentId, string machineId)
        {
            ThreadId = threadId;
            AgentId = agentId;
            MachineId = machineId;
        }

        public string MachineId { get; private set; }
        public string AgentId { get; private set; }
        public string ThreadId { get; private set; }

        public override string ToString()
        {
            return string.Format("Agent '{0}', Machine '{1}', Thread '{2}'", AgentId, MachineId, ThreadId);
        }
    }
}