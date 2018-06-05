using MemoryAnalyzer.Analyzer;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MemoryAnalyzer.Forms
{
    /// <summary>
    /// Dedicated thread listening on queue for incoming commands
    /// </summary>
    internal class CommandQueue
    {
        private BlockingCollection<Command> queue;
        private Thread queueThread;

        public CommandQueue()
        {
            queue = new BlockingCollection<Command>();
            queueThread = new Thread(new ThreadStart(Run));
            queueThread.Start();
        }

        /// <summary>
        /// Adds given command to queue
        /// </summary>
        /// <param name="command"></param>
        internal void Add(Command command)
        {
            queue.Add(command);
            command.Status = Command.CommandStatus.QUEUED;
        }

        private void Run()
        {
            Command cmd = null;
            while ((cmd = queue.Take()) != null)
            {
                // process request
                cmd.Status = Command.CommandStatus.RUNNING;
                CommandExecutioner.Execute(cmd);
            }
        }
    }
}