using System.Collections.Generic;

namespace WebService.Models
{
    public class ResidentUpdater
    {
        public Resident Resident { get; set; }
        public IEnumerable<string> PropertiesToUpdate { get; set; }
    }
}
