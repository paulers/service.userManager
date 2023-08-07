using Service.UserManager.Models;
using Service.UserManager.Repositories;
using Service.UserManager.ViewModel;

namespace Service.UserManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserAccountRepository _userAccountRepository;

        public AccountController(ILogger<AccountController> logger, IUserAccountRepository userAccountRepository)
        {
            _logger = logger;
            _userAccountRepository = userAccountRepository;
        }

        [HttpPost]
        public async Task<ActionResult<UserAccount>> CreateUser([FromBody] CreateUserViewModel model)
        {
            try
            {
                var createdUser = await _userAccountRepository.CreateUser(model);
                Utilities.SanitizeUserAccountModel(createdUser);
                return createdUser;
            } catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error while creating user: {ex.Message}");
                return StatusCode(400, $"Error occurred while creating user. Our staff has been notified.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserAccount>> GetUser([FromRoute] Guid id)
        {
            try
            {
                var model = await _userAccountRepository.GetUser(id);
                if (model == null) return NotFound();
                Utilities.SanitizeUserAccountModel(model);
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error while fetching user: {ex.Message}");
                return StatusCode(400, $"Error occurred while fetching user. Our staff has been notified.");
            }
        }
    }
}