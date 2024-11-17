using Citizen_E_Tax_API.DataAccess.Interface;
using Citizen_E_Tax_API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Citizen_E_Tax_API.Controllers
{
    public class Tax_API_Controller : Controller
    {
        private readonly ITaxRepository _repo;
        public Tax_API_Controller(ITaxRepository repo)
        {
            _repo = repo;
        }
        
        [HttpPost("RegisterTaxAdmin")]
        public async Task<IActionResult> RegisterAdmin (RegisterTx _reg)
        {
            var res = await _repo.RegisterTaxAdmin(_reg);
            if (res.IsSuccess)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }

        }
        [HttpPost("RegisterTaxMember")]
        public async Task<IActionResult> RegisterMember(RegisterTx _reg)
        {
            var res = await _repo.RegisterTaxOwner(_reg);
            if (res.IsSuccess)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }

        }
        [HttpPost("Login")]
        public async Task<IActionResult>Login(LoginTx login)
        {
            try
            {
                var res = await _repo.Login(login);
                return Ok(res);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("VerifyDocument")]
        public async Task<IActionResult> VerifyDocuments(Guid id)
        {
            try
            {
                var res = await _repo.VerifyDocuments(id);
                return Ok(res);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("PayTax")]
        public async Task<IActionResult>PayTax(PayTaxDTO payTaxDTO)
        {
            try
            {
                var res = await _repo.PayTax(payTaxDTO);
                return Ok(res); 
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("CalculateTax")]
        public async Task<IActionResult> CalculateTax(Guid id)
        {
            try
            {
                var res = await _repo.CalculateTaxBasedOnIncome(id);
                return Ok(res);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("CheckTaxPAymentHistory")]
        public async Task<IActionResult> CheckHistory(Guid id)
        {
            try
            {
                var res = await _repo.CheckTaxHistory(id);
                return Ok(res);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
