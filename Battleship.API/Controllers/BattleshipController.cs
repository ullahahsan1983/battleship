using Battleship.Core;
using Battleship.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BattleshipController : ControllerBase
    {
        private readonly ILogger<BattleshipController> _logger;
        private readonly IGameEngine _gameEngine;
        private readonly IGameEnvironment _gameEnvironment;

        public BattleshipController(
            ILogger<BattleshipController> logger,
            IGameEngine gameEngine,
            IGameEnvironment gameEnvironment)
        {
            _logger = logger;
            _gameEngine = gameEngine;
            _gameEnvironment = gameEnvironment;
        }

        [HttpPost("StartNewGame")]
        public GameReport StartNewGame()
        {
            _gameEngine.StartNewGame();

            return _gameEngine.GetReport();
        }

        [HttpPost("Attack")]
        public AttackReport Attack(Coordinate coordinate)
        {
            return _gameEngine.LaunchAttackByPlayer(coordinate);
        }

        [HttpGet("Report")]
        public GameReport GetReport()
        {
            return _gameEngine.GetReport();
        }
    }
}