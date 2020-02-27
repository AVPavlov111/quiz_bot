using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TrueQuizBot.WebApi.Controllers
{
    [Route("api/results")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly IDataProvider _dataProvider;

        public ResultsController(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        [HttpGet, Route("winners/{count}")]
        public async Task<List<Winner>> GetWinners(int count)
        {
            return await _dataProvider.GetWinners(count);
        }
        
        [HttpGet, Route("luckers")]
        public async Task<List<Winner>> GetLuckers()
        {
            return await _dataProvider.GetLuckers();
        }
    }
}