using FluentScheduler;

namespace Rhema.FluentScheduler
{
    public class TaskRegistry : Registry
    {
        public TaskRegistry()
        {
            Schedule<TestJob>().WithName("TestJob").ToRunNow() // Delay startup for a while
                .AndEvery(60).Seconds();

            Schedule<TestJob>().WithName("Test2Job").ToRunNow() // Delay startup for a while
                .AndEvery(30).Seconds();

        }


    }
}
