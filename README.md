# Rhema.FluentScheduler

Rhema.FluentScheduler

For Manual Job

    public void DoJob()
    {
                var eng = Container.Resolve<IScheduleJobEngine>();
                eng.AddJob(() =>
                    {//task...
                        //nothing
                    },
                    s => s.ToRunNow()).JobEnd(new JobEndData<string>() { Data = "1" }, CallbackTask);
                eng.Run();
                eng.Stop();
     }
    public  object CallbackTask(JobEndData<string> info)
    {
                test1expectedval = info.Data;
                info.Stop(info.Info.Name);
                return null;
    
    }