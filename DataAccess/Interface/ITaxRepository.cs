using Citizen_E_Tax_API.Models.Domain;
using Citizen_E_Tax_API.Models.DTOs;
using Citizen_E_Tax_API.Services;

namespace Citizen_E_Tax_API.DataAccess.Interface
{
    public interface ITaxRepository
    {
        public Task<ResponseModel<string>> RegisterTaxOwner(RegisterTx register);
        public Task<ResponseModel<string>> RegisterTaxAdmin(RegisterTx register);
        public Task<ResponseModel<string>> Login(LoginTx login);
        public Task<ResponseModel<string>> VerifyDocuments(Guid Id);
        public Task<ResponseModel<string>> PayTax(PayTaxDTO _pay);
        public Task<ResponseModel<string>> CalculateTaxBasedOnIncome(Guid Id);
        public Task<List<Payment>>CheckTaxHistory(Guid Id);



    }
}
