using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFirstRemote.Core.Models;

namespace TaskFirstRemote.Core.Interfaces
{

    public interface ILogStorage
    {
        Task SaveLogAsync(WeatherLog log);
        Task<IEnumerable<WeatherLog>> GetLogsAsync(DateTime from, DateTime to);
    }
}
