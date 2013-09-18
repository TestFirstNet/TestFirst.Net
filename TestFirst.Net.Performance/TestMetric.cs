using System;

namespace TestFirst.Net.Performance
{
    public class TestId
    {
        public String MachineId{ get; private set; }
        public String AgentId { get; private set; }
        public String ThreadId { get; private set; }
        
        internal TestId(string threadId, string agentId, string machineId)
        {
            ThreadId = threadId;
            AgentId = agentId;
            MachineId = machineId;
        }

        public override String ToString()
        {
            return String.Format("Agent '{0}', Machine '{1}', Thread '{2}'", AgentId, MachineId, ThreadId);
        }

    }
}