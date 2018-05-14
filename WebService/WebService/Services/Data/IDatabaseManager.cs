using System;
using System.Threading.Tasks;

namespace WebService.Services.Data
{
    /// <summary>
    /// An interface that describes the methods to keep the database clean.
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        /// Bool to indicate whether the manager is working.
        /// </summary>
        bool IsWorking { get; }
        
        /// <summary>
        /// Bool to indicate whether the cleanups are scheduled.
        /// </summary>
        bool IsSchedulerRunning { get; }
        
        /// <summary>
        /// TimeSpan that holds the interval in which the cleanup should be done.
        /// </summary>
        TimeSpan SchedulingInterval { get; set; }

        
        /// <summary>
        /// Removes all the relations in the database that relate to a non existing entity.
        /// </summary>
        Task RemoveUnresolvableRelationsAsync();
        
        /// <summary>
        /// Removes al redundant data from the database.
        /// </summary>
        Task RemoveRedundantDataAsync();
        
        /// <summary>
        /// Fills all the missing fields that should be filled in, in the database.
        /// </summary>
        Task FillMissingFieldsAsync();

        
        /// <summary>
        /// Executes all the cleanup methods:
        /// 
        /// <list type="bullet">
        ///     <item><description><see cref="RemoveUnresolvableRelationsAsync"/></description></item>
        ///     <item><description><see cref="RemoveRedundantDataAsync"/></description></item>
        ///     <item><description><see cref="FillMissingFieldsAsync"/></description></item>
        /// </list>
        /// 
        /// </summary>
        void Cleanup();

        
        /// <summary>
        /// Schedules to execute the <see cref="Cleanup"/> method every interval given in the
        /// <see cref="SchedulingInterval"/> property.
        /// </summary>
        /// <returns></returns>
        Task ScheduleCleanup();
        
        /// <summary>
        /// Schedules te execute the <see cref="Cleanup"/> method every interval given in the <see cref="interval"/>
        /// argument.
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        Task ScheduleCleanup(TimeSpan interval);
        
        /// <summary>
        /// Cancels the scheduled cleanups.
        /// </summary>
        void CancelCleanupSchedule();
        
        /// <summary>
        /// Configures the database. It creates the default users.
        /// </summary>
        void ConfigureDB();
    }
}