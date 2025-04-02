using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFirstRemote.Core.Interfaces
{
    public interface IWeatherService
    {
        Task<string> FetchWeatherDataAsync();
    }
}
