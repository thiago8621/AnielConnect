using Hangfire.Logging;

namespace Aniel.Connect.Tasks
{
    public class BackgroundTask
    {
        protected ILog Logger
        {
            get
            {
                return LogProvider.GetLogger("Aniel.Connect.Tasks");
            }
        }
    }
}