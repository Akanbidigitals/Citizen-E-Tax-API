using Citizen_E_Tax_API.DataAccess.DataContext;
using Citizen_E_Tax_API.DataAccess.Interface;
using Citizen_E_Tax_API.Models.Domain;
using Citizen_E_Tax_API.Models.DTOs;
using Citizen_E_Tax_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Citizen_E_Tax_API.DataAccess.Repository
{
    public class TaxRepository : ITaxRepository
    {
        private readonly ApplicationContext _ctx;
        private readonly ILogger<TaxRepository> _logger;
        private readonly AppSettings _appsettings;
        public TaxRepository(ApplicationContext ctx, ILogger<TaxRepository> logger,IOptions<AppSettings> appsettings)
        {
            _appsettings = appsettings.Value;
            _ctx = ctx;
            _logger = logger;   

        }

        public async Task<ResponseModel<string>> CalculateTaxBasedOnIncome(Guid Id)
        {
            var response = new ResponseModel<string>();
            try
            {
                var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Id == Id);
                if (user == null)
                {
                    response = response.FailedResult("User does not exist");
                }
                else
                {
                    if (user.MonthlyIncome < 1000)
                    {
                        var tax = user.MonthlyIncome * 10 / 100;
                        response = response.SuccessResult($"Dear {user.FullName},Your tax monthly is {tax} based on your {user.MonthlyIncome} Monthly Income");


                    }
                    else if (user.MonthlyIncome > 1001 && user.MonthlyIncome < 3000)
                    {
                        var tax = user.MonthlyIncome * 20 / 100;
                        response = response.SuccessResult($"Dear {user.FullName},Your tax monthly is {tax} based on your {user.MonthlyIncome} Monthly Income");
                    }
                    else if (user.MonthlyIncome > 3001)
                    {
                       var tax = user.MonthlyIncome * 30 / 100;
                    response = response.SuccessResult($"Dear {user.FullName},Your tax monthly is {tax} based on your {user.MonthlyIncome} Monthly Income");

                    }



                }

            }catch(Exception ex)
            {
                response = response.FailedResult(ex.Message);
            }
            return response;
        }
        
        public async Task<List<Payment>> CheckTaxHistory(Guid Id)
        {
            try
            {
                var transaction = await _ctx.Payments.ToListAsync();
                if(transaction.Count == 0)
                {
                    throw new Exception("Payment history is empty");
                }
                return transaction;
            }catch( Exception ex )
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<string>> Login(LoginTx login)
        {
            var response = new ResponseModel<string>();
            try
            {
                var user = await _ctx.Users.Include(x => x.Roles).ThenInclude(x => x.Role).FirstOrDefaultAsync(x => x.Email == login.Email);
                var password = Helper.VerifyHashPasswod(login.Password, user.Password);
                if(user == null || !password)
                {
                    response = response.FailedResult("Email or password is incorrect, pls try again");
                }
                else
                {
                    var userRoles = user.Roles.Select(x => x.Role.Name).ToList();
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name,user.FullName),
                        new(ClaimTypes.Email,user.Email)
                    };
                    foreach(var role in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appsettings.SecretKey));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _appsettings.Issuer,
                        signingCredentials: credentials,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(120));
                    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                    response = response.SuccessResult(jwtToken);
                }

            }catch(Exception ex)
            {
                response = response.FailedResult(ex.Message);
            }
            return response;
        }

        public async Task<ResponseModel<string>> PayTax(PayTaxDTO _pay)
        {
            var response = new ResponseModel<string>();
            try
            {
                var user = await _ctx.Users.FirstOrDefaultAsync(x=>x.Id == _pay.Id);
                if(user is null)
                {
                    response = response.FailedResult("User does not exist");
                }
                else
                {
                    var payment = new Payment()
                    {
                        UserId = user.Id,
                        PaymentMonth = _pay.PaymentforMonth,
                        PaymentReference = Helper.GenerateReference().Substring(0,9)
                    };
                    await _ctx.Payments.AddAsync(payment);
                    await _ctx.SaveChangesAsync();
                }

            }catch(Exception ex)
            {
                response = response.FailedResult(ex.Message);
            }
            return response;
        }

        public async Task<ResponseModel<string>> RegisterTaxAdmin(RegisterTx register)
        {

            var response = new ResponseModel<string>();
            var maxFileSize = _appsettings.MaxFileSize * 1024 * 1024;
            if(register.IdentificationDocument.Length > maxFileSize)
            {
                _logger.LogError("File zise has exceeded allowed limit");
                response =  response.FailedResult($"File size has exceeded allowed limit of {_appsettings.MaxFileSize}mb");

            }
           // Using extension
            var fileExtension = Path.GetExtension(register.IdentificationDocument.FileName);
            if (!fileExtension.ToLower().Equals(".pdf") && !fileExtension.ToLower().Equals(".doc") && !fileExtension.ToLower().Equals("docx"))
            {
                response = response.FailedResult("Only PDF or Word documments are allowed");
            }

            //   Using Mime type / Content
            //var fileExtension = new List<string>
            //{
            //     "application/msword",
            //    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            //    "application/pdf"

            //};
            //if (!fileExtension.Contains(register.IdentificationDocument.ContentType))
            //{
            //    response = response.FailedResult("Only PDF or Word documents are allowed");
            //}
            try
            {
                var user = await _ctx.Users.AnyAsync(x => x.Email == register.Email);
                if (user)
                {
                    response = response.FailedResult("Email already exist");
                    _logger.LogError("Email already exist");
                }
                var folderPath = Path.Combine(_appsettings.StoragePath, register.Email); //Documents/akanbi@gmail.com
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, register.IdentificationDocument.FileName.ToLower());
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await register.IdentificationDocument.CopyToAsync(stream);
                }
                var addNewUser = new User()
                {
                    FullName = register.FullName,
                    Email = register.Email,
                    Password = Helper.HashPassword(register.Password),
                    PhoneNumber = register.PhoneNumber,
                    Documents = []

                };
                 
                await _ctx.Users.AddAsync(addNewUser);
               var res = await _ctx.SaveChangesAsync();

                if (res > 0)
                {
                    var role = await _ctx.Roles.FirstOrDefaultAsync(x => x.Id == 1);

                    var userRole = new UserRole()
                    {
                        RoleId = role.Id,
                        Role = role,
                        User = addNewUser,
                        UserId = addNewUser.Id
                    };

                    addNewUser.Roles.Add(userRole);
                    await _ctx.SaveChangesAsync();


                    var document = new Document()
                    {
                        Name = register.IdentificationDocument.FileName,
                        Path = filePath,
                        UserId = addNewUser.Id,
                        Extension = Path.GetExtension(register.IdentificationDocument.FileName),
                        UploadedOn = DateTime.UtcNow,

                    };
                    await _ctx.Documents.AddAsync(document);
                    await _ctx.SaveChangesAsync();
                }   
                    response = response.SuccessResult($"{addNewUser}");
            }catch(Exception ex)
            {
                response = response.FailedResult(ex.Message);
            }
            return response;
        }

        public async Task<ResponseModel<string>> RegisterTaxOwner(RegisterTx register)
        {
            var response = new ResponseModel<string>();
            var maxFileSize = _appsettings.MaxFileSize * 1024 * 1024;
            if (register.IdentificationDocument.Length > maxFileSize)
            {
                _logger.LogError("File zise has exceeded allowed limit");
                return response.FailedResult($"File size has exceeded allowed limit of {_appsettings.MaxFileSize}mb");

            }
            var fileExtension = new List<string>
            {
                 "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/pdf"

            };
            if (!fileExtension.Contains(register.IdentificationDocument.ContentType))
            {
                return response.FailedResult("Only PDF or Word documents are allowed");
            }
            try
            {
                var user = await _ctx.Users.AnyAsync(x => x.Email == register.Email);
                if (user)
                {
                    response = response.FailedResult("Email already exist");
                    _logger.LogError("Email already exist");
                }
                var folderPath = Path.Combine(_appsettings.StoragePath, register.Email); //Documents/akanbi@gmail.com
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, register.IdentificationDocument.FileName.ToLower());
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await register.IdentificationDocument.CopyToAsync(stream);
                }
                var addNewUser = new User()
                {
                    Email = register.Email,
                    Password = Helper.HashPassword(register.Password),
                    PhoneNumber = register.PhoneNumber,
                    FullName = register.FullName,
                    TaxIdentificationNumber = Helper.GenerateTxnNumber(),
                    IdentificationNumber = register.IdentificationNumber,
                    MonthlyIncome = register.MonthlyIncome,
                    Documents = []
                };

                await _ctx.Users.AddAsync(addNewUser);
                var res = await _ctx.SaveChangesAsync();
                if (res > 0)
                {
                    var role = await _ctx.Roles.FirstOrDefaultAsync(x => x.Id == 2);

                    var userRole = new UserRole()
                    {
                        RoleId = role.Id,
                        Role = role,
                        UserId = addNewUser.Id,
                        User = addNewUser
                    };

                    addNewUser.Roles.Add(userRole);
                    await _ctx.SaveChangesAsync();
                }


                var document = new Document()
                {
                    Name = register.IdentificationDocument.FileName,
                    Path = filePath,
                    UserId = addNewUser.Id,
                    Extension = Path.GetExtension(register.IdentificationDocument.FileName),
                    UploadedOn = DateTime.UtcNow,

                };
                addNewUser.Documents.Add(document);
                await _ctx.SaveChangesAsync();

                response = response.SuccessResult($"{addNewUser}");

            }
            catch (Exception ex)
            {
                response = response.FailedResult(ex.Message);
            }
            return response;
        }

        public async Task<ResponseModel<string>> VerifyDocuments(Guid Id)
        {
            var response = new ResponseModel<string>();
            try
            {
                var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Id == Id);
                if(user is null)
                {
                    response = response.FailedResult("User does not exist");
                }
                user.Isverified = true;
                _ctx.Users.Update(user);
                await _ctx.SaveChangesAsync();

            }catch(Exception ex)
            {
                response = response.FailedResult(ex.Message);
            }
            return response;
        }
    }
}
