using Application.DTO.Account;
using Application.Infrastructure.Result;
using Application.Logic.Account;
using Application.ViewModel.Account;
using AutoMapper;
using Domain.Contracts;
using Domain.Contracts.Account;
using Infrastructure.UserRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDTO UserDto)
        {
            // Generate password
            string userPassword = UserDto.Password;
            // Register new user
            ApplicationUser user = await _accountService.Create(UserDto, userPassword);
            // Done
            return Created("", new OperationResult() { Success = true, Result = new { user = _mapper.Map<UserViewModel>(user) } });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserEditDTO user)
        {
            var editedUser = await _accountService.Edit(id, user);
            return Ok(new OperationResult() { Success = true, Result = new { user = _mapper.Map<UserViewModel>(editedUser) } });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool result = await _accountService.Delete(id);
            return Ok(new OperationResult() { Success = true, Result = new { message = "User account was deleted." } });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            // 2. Get user data
            var user = await _accountService.GetById(id);
            return Ok(new OperationResult() { Success = true, Result = new { user = _mapper.Map<UserViewModel>(user) } });
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterUserDTO filterUserDTO)
        {
            // 1. Filter
            var queryFilter = _mapper.Map<UserQueryFilter>(filterUserDTO);
            // 2. Get available users
            var users = await _accountService.GetAll(queryFilter);
            // 3. Prepare Result
            int totalItems = await _accountService.CountAll(queryFilter);
            var result = new
            {
                users = _mapper.Map<List<UserViewModel>>(users.ToList()),
                pageNumber = filterUserDTO.PageNumber,
                pageSize = filterUserDTO.PageSize,
                totalItems = totalItems,
                totalPages = (int)Math.Ceiling(totalItems / (double)filterUserDTO.PageSize)
            };
            return Ok(new OperationResult() { Success = true, Result = result });
        }
    }
}
