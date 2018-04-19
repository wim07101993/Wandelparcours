using System.Threading.Tasks;

namespace WebService.Services.Data
{
    public interface IDatabaseManager
    {
        bool IsWorking { get; }
        bool IsSchedulerRunning { get; }
        int SchedulingInterval { get; set; }

        Task RemoveUnresolvableRelations();
        Task RemoveRedundantData();
        Task FillMissingFields();

        void Cleanup();

        Task ScheduleCleanup();
        Task ScheduleCleanup(int interval);
        void CancelCleanupSchedule();
    }
}