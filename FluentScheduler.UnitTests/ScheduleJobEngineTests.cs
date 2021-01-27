using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Rhema.FluentScheduler;
using Serilog;

namespace FluentScheduler.UnitTests
{
    using System;
    using Xunit;
    using static Xunit.Assert;

    public class ScheduleJobEngineTests
    {
        public static IContainer Container { get; set; }
        public ScheduleJobEngineTests()
        {
            var _container = IoC.InitializeAutoFacContainer();
            JobManager.JobFactory = new RhemaJobFactory();
            Container = _container;
        }

        [Fact]
        public void Should_ScheduleJobEngine_instantized()
        {
            // Arrange
            var _container = IoC.InitializeAutoFacContainer();
            JobManager.JobFactory = new RhemaJobFactory();


            // Act
            var eng = _container.Resolve<IScheduleJobEngine>();
            var eng2 = ContextEngine.Current.Resolve<IScheduleJobEngine>();

            // Assert
            NotNull(eng2);
            NotNull(eng);

        }

        [Fact]
        public void Should_JobEngine_WithRegistry_WORKED()
        {
            // Arrange
            var input = new DateTime(2000, 1, 1, 1, 23, 0);
            var expected = new DateTime(2000, 1, 1, 1, 24, 0);
            var eng = Container.Resolve<IScheduleJobEngine>();
            // Act
            eng.WithRegistry(new TaskRegistry());

            var s = JobManager.GetSchedule("TestJob");
            var actual = s.CalculateNextRun(input);

            //Assert
            Equal(expected, actual);
         
        }
        [Fact]
        public void Should_JobEngine_WithRegistry_JobEnd_WORKED()
        {
            // Arrange
            var eng = Container.Resolve<IScheduleJobEngine>();
            // Act
            eng.WithRegistry(new TaskRegistry()).JobEnd("expected", Test2);
            JobManager.StopAndBlock();

            //Assert
            Equal(2, test2ActionTimers);
            Equal("expected", test2expectedval);
        }

        [Fact]
        public void Should_JobEngine_AddJob_WORKED()
        {
            // Arrange
            var eng = Container.Resolve<IScheduleJobEngine>();
            var expected = new TimeSpan(0,0,1,0);
            // Act
            eng .AddJob(() =>
                {
                  //nothing
                },
                s => s.WithName("TestJob7").ToRunNow().DelayFor(1).Minutes());
        
            //Assert
            var s = JobManager.GetSchedule("TestJob7");
            var actual = s.DelayRunFor;

            //Assert
            Equal(expected, actual);
        }
        [Fact]
        public void Should_JobEngine_AddJob_JobEnd_WORKED()
        {
            // Arrange
            var eng = Container.Resolve<IScheduleJobEngine>();
            // Act
            eng.AddJob(() =>
                {
                    //nothing
                },
                s => s.WithName("TestJob8").ToRunNow()).JobEnd("1", Test1);

            JobManager.StopAndBlock();
            //Assert
            Equal("1", test1expectedval);
        }

        public  object Test1(RhemaJobEndInfo info, object val)
        {
            test1expectedval = val.ToString();
            info.Stop(info.JobName);

            return null;

        }
        public int test2ActionTimers = 0;
        public string test2expectedval = "";
        public string test1expectedval = "";
        private void Test2(RhemaJobEndInfo info, object val)
        {
            test2ActionTimers++;
            test2expectedval = val.ToString();
          
            info.Stop(info.JobName);

        }

    }
}
