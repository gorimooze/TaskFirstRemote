using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFirstRemote.Core.Models
{
    public class WeatherLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Success";
        public string? ErrorMessage { get; set; }
        public string? BlobName { get; set; }
    }
}
