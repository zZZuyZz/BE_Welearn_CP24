using DataLayer.DbObject;
using ServiceLayer.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ServiceLayer.Services.Interface;

namespace APIExtension.Validator
{
    //public interface IAccountValidator
    //{
    //    Task<ValidatorResult> ValidateParams(int accountId, AccountUpdateDto dto);
    //    Task<ValidatorResult> ValidateParams(StudentRegisterDto dto);
    //    Task<ValidatorResult> ValidateParams(ParentRegisterDto dto);
    //    Task<ValidatorResult> ValidateParams(int accountId, AccountChangePasswordDto dto);
    //}
    //public class AccountValidator : BaseValidator, IAccountValidator
    //{
    //    private IServiceWrapper services;

    //    public AccountValidator(IServiceWrapper services)
    //    {
    //        this.services = services;
    //    }
    //    //^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$
    //    //^[0-9]{8,20}$
    //    Regex phoneRegex = new Regex(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$");

    //    public async Task<ValidatorResult> ValidateParams(StudentRegisterDto dto)
    //    {
    //        try
    //        {
    //            //username
    //            if (await services.Accounts.ExistUsernameAsync(dto.Username))
    //            {
    //                //validatorResult.Failures.Add("Tên tài khoản đã tồn tại");
    //                validatorResult.Add("Tên tài khoản đã tồn tại", nameof(dto.Username));
    //            }
    //            if (dto.Username.Trim().Length == 0)
    //            {
    //                //validatorResult.Failures.Add("Thiếu tên tài khoản");
    //                validatorResult.Add("Thiếu tên tài khoản", nameof(dto.Username));
    //            }
    //            if (dto.Username.Trim().Length > 32)
    //            {
    //                //validatorResult.Failures.Add("Tên tài khoản quá dài");
    //                validatorResult.Add("Tên tài khoản quá dài", nameof(dto.Username));
    //            }
    //            //email                                                   
    //            if (await services.Accounts.ExistEmailAsync(dto.Email))
    //            {
    //                // validatorResult.Failures.Add("Email đã tồn tại");
    //                validatorResult.Add("Email đã tồn tại", nameof(dto.Email));
    //            }
    //            if (dto.Email.Trim().Length == 0)
    //            {
    //                // validatorResult.Failures.Add("Thiếu email");
    //                validatorResult.Add("Thiếu email", nameof(dto.Email));
    //            }
    //            //password   
    //            if (dto.Password.Trim().Length == 0)
    //            {
    //                // validatorResult.Failures.Add("Thiếu mật khẩu");
    //                validatorResult.Add("Thiếu mật khẩu", nameof(dto.Password));
    //            }
    //            if (dto.Password.Length > 32)
    //            {
    //                // validatorResult.Failures.Add("Mật khẩu quá dài");
    //                validatorResult.Add("Mật khẩu quá dài", nameof(dto.Password));
    //            }
    //            if (dto.Password != dto.ConfirmPassword)
    //            {
    //                // validatorResult.Failures.Add("Xác nhận mật khẩu không thành công");
    //                validatorResult.Add("Xác nhận mật khẩu không thành công", nameof(dto.Password));
    //            }
    //            //name
    //            if (dto.FullName.Trim().Length == 0)
    //            {
    //                // validatorResult.Failures.Add("Thiếu họ tên");
    //                validatorResult.Add("Thiếu họ tên", nameof(dto.FullName));
    //            }
    //            if (dto.FullName.Trim().Length > 50)
    //            {
    //                // validatorResult.Failures.Add("Họ tên quá dài");
    //                validatorResult.Add("Họ tên quá dài", nameof(dto.FullName));
    //            }
    //            //sđt
    //            if (dto.Phone.Trim().Length == 0)
    //            {
    //                // validatorResult.Failures.Add("Thiếu số điện thoại");
    //                validatorResult.Add("Thiếu số điện thoại", nameof(dto.Phone));
    //            }
    //            if (dto.Phone.Trim().Length > 20)
    //            {
    //                // validatorResult.Failures.Add("Số điện thoại quá dài");
    //                validatorResult.Add("Số điện thoại quá dài", nameof(dto.Phone));
    //            }
    //            if (!phoneRegex.IsMatch(dto.Phone))
    //            {
    //                // validatorResult.Failures.Add("Số điện thoại không đúng định dạng");
    //                validatorResult.Add("Số điện thoại không đúng định dạng", nameof(dto.Phone));
    //            }
    //            //Dob
    //            if (dto.DateOfBirth != null)
    //            {
    //                if (dto.DateOfBirth.Value > DateTime.Today)
    //                {
    //                    // validatorResult.Failures.Add("Ngày sinh không hợp lệ");
    //                    validatorResult.Add("Ngày sinh không hợp lệ", nameof(dto.DateOfBirth));
    //                }
    //            }
    //            //School
    //            //if (dto.Schhool != null)
    //            //{
    //            //    if (dto.Schhool.Trim().Length == 0)
    //            //    {
    //            //       // validatorResult.Failures.Add("Thiếu tên trường");
    //            //    }
    //            //}
    //        }

    //        catch (Exception ex)
    //        {
    //            //validatorResult.Failures.Add(ex.ToString());
    //            validatorResult.Add(ex.ToString());
    //        }
    //        return validatorResult;
    //    }

    //    public async Task<ValidatorResult> ValidateParams(ParentRegisterDto dto)
    //    {
    //        try
    //        {
    //            //username
    //            if (await services.Accounts.ExistUsernameAsync(dto.Username))
    //            {
    //                // validatorResult.Failures.Add("Tên tài khoản đã tồn tại");
    //                validatorResult.Add("Tên tài khoản đã tồn tại", nameof(dto.Username));
    //            }
    //            if (dto.Username.Trim().Length == 0)
    //            {
    //                // validatorResult.Failures.Add("Thiếu tên tài khoản");
    //                validatorResult.Add("Thiếu tên tài khoản", nameof(dto.Username));
    //            }
    //            if (dto.Username.Trim().Length > 32)
    //            {
    //                // validatorResult.Failures.Add("Tên tài khoản quá dài");
    //                validatorResult.Add("Tên tài khoản quá dài", nameof(dto.Username));
    //            }
    //            //email                                                   
    //            if (await services.Accounts.ExistEmailAsync(dto.Email))
    //            {
    //                // validatorResult.Failures.Add("Email đã tồn tại");
    //                validatorResult.Add("Email đã tồn tại", nameof(dto.Email));
    //            }
    //            if (dto.Email.Trim().Length == 0)
    //            {
    //                // validatorResult.Failures.Add("Thiếu email");
    //                validatorResult.Add("Thiếu email", nameof(dto.Email));
    //            }
    //            //password   
    //            if (dto.Password.Trim().Length == 0)
    //            {
    //                // validatorResult.Failures.Add("Thiếu mật khẩu");
    //                validatorResult.Add("Thiếu mật khẩu", nameof(dto.Password));
    //            }
    //            if (dto.Password.Length > 32)
    //            {
    //                // validatorResult.Failures.Add("Mật khẩu quá dài");
    //                validatorResult.Add("Mật khẩu quá dài", nameof(dto.Password));
    //            }
    //            if (dto.Password != dto.ConfirmPassword)
    //            {
    //                // validatorResult.Failures.Add("Xác nhận mật khẩu không thành công");
    //                validatorResult.Add("Xác nhận mật khẩu không thành công", nameof(dto.Password));
    //            }
    //            //name
    //            if (dto.FullName.Trim().Length == 0)
    //            {
    //                // validatorResult.Failures.Add("Xác nhận mật khẩu không thành công");
    //                validatorResult.Add("Xác nhận mật khẩu không thành công", nameof(dto.FullName));
    //            }
    //            if (dto.FullName.Trim().Length > 50)
    //            {
    //                // validatorResult.Failures.Add("Họ tên quá dài");
    //                validatorResult.Add("Họ tên quá dài", nameof(dto.FullName));
    //            }
    //            //sđt
    //            if (dto.Phone.Trim().Length == 0)
    //            {
    //                // validatorResult.Failures.Add("Thiếu số điện thoại");
    //                validatorResult.Add("Thiếu số điện thoại", nameof(dto.Phone));
    //            }
    //            if (dto.Phone.Trim().Length > 20)
    //            {
    //                // validatorResult.Failures.Add("Số điện thoại quá dài");
    //                validatorResult.Add("Số điện thoại quá dài", nameof(dto.Phone));
    //            }
    //            if (!phoneRegex.IsMatch(dto.Phone))
    //            {
    //                // validatorResult.Failures.Add("Số điện thoại không đúng định dạng");
    //                validatorResult.Add("Số điện thoại không đúng định dạng", nameof(dto.Phone));
    //            }
    //            //Dob
    //            if (dto.DateOfBirth != null)
    //            {
    //                if (dto.DateOfBirth.Value > DateTime.Today)
    //                {
    //                    // validatorResult.Failures.Add("Ngày sinh không hợp lệ");
    //                    validatorResult.Add("Ngày sinh không hợp lệ", nameof(dto.DateOfBirth));
    //                }
    //            }
    //        }

    //        catch (Exception ex)
    //        {
    //            // validatorResult.Failures.Add(ex.ToString());
    //            validatorResult.Add(ex.ToString());
    //        }
    //        return validatorResult;
    //    }

    //    public async Task<ValidatorResult> ValidateParams(int accountId, AccountUpdateDto dto)
    //    {
    //        try
    //        {
    //            if (!await services.Accounts.ExistAsync(accountId))
    //            {
    //                // validatorResult.Failures.Add("Tài khoản không tồn tại");
    //                validatorResult.Add("Tài khoản không tồn tại", ValidateErrType.NotFound);
    //            }
    //            //Nếu null thì ko update
    //            if (dto.FullName != null && dto.FullName.Trim().Length == 0)
    //            {
    //                // validatorResult.Failures.Add("Thiếu họ tên");
    //                validatorResult.Add("Thiếu họ tên", nameof(dto.FullName));
    //            }
    //            if (dto.FullName != null && dto.FullName.Trim().Length > 50)
    //            {
    //                // validatorResult.Failures.Add("Họ tên quá dài");
    //                validatorResult.Add("Họ tên quá dài", nameof(dto.FullName));
    //            }
    //            if (dto.Phone != null && dto.Phone.Trim().Length == 0)
    //            {
    //                // validatorResult.Failures.Add("Thiếu số điện thoại");
    //                validatorResult.Add("Thiếu số điện thoại", nameof(dto.Phone));
    //            }
    //            if (dto.Phone != null && !phoneRegex.IsMatch(dto.Phone))
    //            {
    //                // validatorResult.Failures.Add("Số điện thoại không đúng định dạng");
    //                validatorResult.Add("Số điện thoại không đúng định dạng", nameof(dto.Phone));
    //            }
    //            //Dob
    //            if (dto.DateOfBirth != null)
    //            {
    //                if (dto.DateOfBirth.Value > DateTime.Today)
    //                {
    //                    // validatorResult.Failures.Add("Ngày sinh không hợp lệ");
    //                    validatorResult.Add("Ngày sinh không hợp lệ", nameof(dto.DateOfBirth));
    //                }
    //            }
    //        }

    //        catch (Exception ex)
    //        {
    //            // validatorResult.Failures.Add(ex.ToString());
    //            validatorResult.Add(ex.ToString());
    //        }
    //        return validatorResult;
    //    }
    //    private static string FAIL_CONFIRM_PASSWORD_MSG => "Xác nhận mật khẩu thất bại";

    //    public async Task<ValidatorResult> ValidateParams(int accountId, AccountChangePasswordDto dto)
    //    {
    //        try
    //        {
    //            if (dto.Password != dto.ConfirmPassword)
    //            {
    //                // validatorResult.Failures.Add(FAIL_CONFIRM_PASSWORD_MSG);
    //                validatorResult.Add(FAIL_CONFIRM_PASSWORD_MSG, nameof(dto.ConfirmPassword));
    //            }
    //            var account = await services.Accounts.GetByIdAsync<Account>(accountId);
    //            if (account == null)
    //            {
    //                // validatorResult.Failures.Add("Tài khoản không tồn tại");
    //                validatorResult.Add("Tài khoản không tồn tại", ValidateErrType.NotFound);
    //                //return NotFound();
    //            }
    //            if (dto.OldPassword != account.Password)
    //            {
    //                // validatorResult.Failures.Add("Nhập mật khẩu cũ thất bại");
    //                validatorResult.Add("Nhập mật khẩu cũ thất bại", nameof(dto.Password));
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // validatorResult.Failures.Add(ex.ToString());
    //            validatorResult.Add(ex.ToString());
    //        }
    //        return validatorResult;
    //    }
    //}
    public static class AccountValidatorExtension
    {
        //^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$
        //^[0-9]{8,20}$
        static Regex phoneRegex = new Regex(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$");

        public static async Task<ValidatorResult> ValidateParams(this ValidatorResult validatorResult, IServiceWrapper services, StudentRegisterDto dto)
        {
            try
            {
                //username
                if(await services.Accounts.ExistUsernameAsync(dto.Username))
                {
                    //validatorResult.Failures.Add("Tên tài khoản đã tồn tại");
                    validatorResult.Add("Tên tài khoản đã tồn tại", nameof(dto.Username));
                }
                if (dto.Username.Trim().Length == 0)
                {
                    //validatorResult.Failures.Add("Thiếu tên tài khoản");
                    validatorResult.Add("Thiếu tên tài khoản", nameof(dto.Username));
                }
                if (dto.Username.Trim().Length > 32)
                {
                    //validatorResult.Failures.Add("Tên tài khoản quá dài");
                    validatorResult.Add("Tên tài khoản quá dài", nameof(dto.Username));
                }
                //email                                                   
                if (await services.Accounts.ExistEmailAsync(dto.Email))
                {
                   // validatorResult.Failures.Add("Email đã tồn tại");
                    validatorResult.Add("Email đã tồn tại", nameof(dto.Email));
                }
                if (dto.Email.Trim().Length == 0)
                {
                   // validatorResult.Failures.Add("Thiếu email");
                    validatorResult.Add("Thiếu email", nameof(dto.Email));
                }
                //password   
                if (dto.Password.Trim().Length == 0)
                {
                   // validatorResult.Failures.Add("Thiếu mật khẩu");
                    validatorResult.Add("Thiếu mật khẩu", nameof(dto.Password));
                }
                if (dto.Password.Length > 32)
                {
                   // validatorResult.Failures.Add("Mật khẩu quá dài");
                    validatorResult.Add("Mật khẩu quá dài", nameof(dto.Password));
                }
                if (dto.Password != dto.ConfirmPassword)
                {
                   // validatorResult.Failures.Add("Xác nhận mật khẩu không thành công");
                    validatorResult.Add("Xác nhận mật khẩu không thành công", nameof(dto.Password));
                }
                //name
                if (dto.FullName.Trim().Length == 0)
                {
                   // validatorResult.Failures.Add("Thiếu họ tên");
                    validatorResult.Add("Thiếu họ tên", nameof(dto.FullName));
                }
                if (dto.FullName.Trim().Length > 50)
                {
                   // validatorResult.Failures.Add("Họ tên quá dài");
                    validatorResult.Add("Họ tên quá dài", nameof(dto.FullName));
                }
                //sđt
                if (dto.Phone.Trim().Length == 0)
                {
                   // validatorResult.Failures.Add("Thiếu số điện thoại");
                    validatorResult.Add("Thiếu số điện thoại", nameof(dto.Phone));
                }
                if (dto.Phone.Trim().Length > 20)
                {
                   // validatorResult.Failures.Add("Số điện thoại quá dài");
                    validatorResult.Add("Số điện thoại quá dài", nameof(dto.Phone));
                }
                if (!phoneRegex.IsMatch(dto.Phone))
                {
                   // validatorResult.Failures.Add("Số điện thoại không đúng định dạng");
                    validatorResult.Add("Số điện thoại không đúng định dạng", nameof(dto.Phone));
                }
                //Dob
                if (dto.DateOfBirth != null)
                {
                    if(dto.DateOfBirth.Value > DateTime.Today)
                    {
                       // validatorResult.Failures.Add("Ngày sinh không hợp lệ");
                        validatorResult.Add("Ngày sinh không hợp lệ", nameof(dto.DateOfBirth));
                    }
                }
                //School
                //if (dto.Schhool != null)
                //{
                //    if (dto.Schhool.Trim().Length == 0)
                //    {
                //       // validatorResult.Failures.Add("Thiếu tên trường");
                //    }
                //}
            }

            catch (Exception ex)
            {
                //validatorResult.Failures.Add(ex.ToString());
                validatorResult.Add(ex.ToString());
            }
            return validatorResult;
        }

        public static async Task<ValidatorResult> ValidateParams(this ValidatorResult validatorResult, IServiceWrapper services, ParentRegisterDto dto)
        {
            try
            {
                //username
                if (await services.Accounts.ExistUsernameAsync(dto.Username))
                {
                   // validatorResult.Failures.Add("Tên tài khoản đã tồn tại");
                    validatorResult.Add("Tên tài khoản đã tồn tại", nameof(dto.Username));
                }
                if (dto.Username.Trim().Length == 0)
                {
                   // validatorResult.Failures.Add("Thiếu tên tài khoản");
                    validatorResult.Add("Thiếu tên tài khoản", nameof(dto.Username));
                }
                if (dto.Username.Trim().Length > 32)
                {
                   // validatorResult.Failures.Add("Tên tài khoản quá dài");
                    validatorResult.Add("Tên tài khoản quá dài", nameof(dto.Username));
                }
                //email                                                   
                if (await services.Accounts.ExistEmailAsync(dto.Email))
                {
                   // validatorResult.Failures.Add("Email đã tồn tại");
                    validatorResult.Add("Email đã tồn tại", nameof(dto.Email));
                }
                if (dto.Email.Trim().Length == 0)
                {
                   // validatorResult.Failures.Add("Thiếu email");
                    validatorResult.Add("Thiếu email", nameof(dto.Email));
                }
                //password   
                if (dto.Password.Trim().Length == 0)
                {
                   // validatorResult.Failures.Add("Thiếu mật khẩu");
                    validatorResult.Add("Thiếu mật khẩu", nameof(dto.Password));
                }
                if (dto.Password.Length > 32)
                {
                   // validatorResult.Failures.Add("Mật khẩu quá dài");
                    validatorResult.Add("Mật khẩu quá dài", nameof(dto.Password));
                }
                if (dto.Password != dto.ConfirmPassword)
                {
                   // validatorResult.Failures.Add("Xác nhận mật khẩu không thành công");
                    validatorResult.Add("Xác nhận mật khẩu không thành công", nameof(dto.Password));
                }
                //name
                if (dto.FullName.Trim().Length == 0)
                {
                   // validatorResult.Failures.Add("Xác nhận mật khẩu không thành công");
                    validatorResult.Add("Xác nhận mật khẩu không thành công", nameof(dto.FullName));
                }
                if (dto.FullName.Trim().Length > 50)
                {
                   // validatorResult.Failures.Add("Họ tên quá dài");
                    validatorResult.Add("Họ tên quá dài", nameof(dto.FullName ));
                }
                //sđt
                if (dto.Phone.Trim().Length == 0)
                {
                   // validatorResult.Failures.Add("Thiếu số điện thoại");
                    validatorResult.Add("Thiếu số điện thoại", nameof(dto.Phone));
                }
                if (dto.Phone.Trim().Length > 20)
                {
                   // validatorResult.Failures.Add("Số điện thoại quá dài");
                    validatorResult.Add("Số điện thoại quá dài", nameof(dto.Phone));
                }
                if (!phoneRegex.IsMatch(dto.Phone))
                {
                   // validatorResult.Failures.Add("Số điện thoại không đúng định dạng");
                    validatorResult.Add("Số điện thoại không đúng định dạng", nameof(dto.Phone));
                }
                //Dob
                if (dto.DateOfBirth != null)
                {
                    if (dto.DateOfBirth.Value > DateTime.Today)
                    {
                       // validatorResult.Failures.Add("Ngày sinh không hợp lệ");
                        validatorResult.Add("Ngày sinh không hợp lệ", nameof(dto.DateOfBirth));
                    }
                }
            }

            catch (Exception ex)
            {
                // validatorResult.Failures.Add(ex.ToString());
                validatorResult.Add(ex.ToString());
            }
            return validatorResult;
        }

        public static async Task<ValidatorResult> ValidateParams(this ValidatorResult validatorResult, IServiceWrapper services, AccountUpdateDto dto, int accountId)
        {
            try
            {
                if (!await services.Accounts.ExistAsync(accountId))
                {
                   // validatorResult.Failures.Add("Tài khoản không tồn tại");
                    validatorResult.Add("Tài khoản không tồn tại", ValidateErrType.NotFound);
                }
                //Nếu null thì ko update
                if (dto.FullName != null && dto.FullName.Trim().Length == 0)
                {
                   // validatorResult.Failures.Add("Thiếu họ tên");
                    validatorResult.Add("Thiếu họ tên", nameof(dto.FullName));
                }
                if (dto.FullName != null && dto.FullName.Trim().Length > 50)
                {
                   // validatorResult.Failures.Add("Họ tên quá dài");
                    validatorResult.Add("Họ tên quá dài", nameof(dto.FullName));
                }
                if (dto.Phone != null && dto.Phone.Trim().Length == 0)
                {
                   // validatorResult.Failures.Add("Thiếu số điện thoại");
                    validatorResult.Add("Thiếu số điện thoại", nameof(dto.Phone));
                }
                if (dto.Phone != null && !phoneRegex.IsMatch(dto.Phone))
                {
                    // validatorResult.Failures.Add("Số điện thoại không đúng định dạng");
                    validatorResult.Add("Số điện thoại không đúng định dạng", nameof(dto.Phone));
                }
                //Dob
                if (dto.DateOfBirth != null)
                {
                    if (dto.DateOfBirth.Value > DateTime.Today)
                    {
                       // validatorResult.Failures.Add("Ngày sinh không hợp lệ");
                        validatorResult.Add("Ngày sinh không hợp lệ", nameof(dto.DateOfBirth));
                    }
                }
            }

            catch (Exception ex)
            {
                // validatorResult.Failures.Add(ex.ToString());
                validatorResult.Add(ex.ToString());
            }
            return validatorResult;
        }
        private static string FAIL_CONFIRM_PASSWORD_MSG => "Xác nhận mật khẩu thất bại";

        public static async Task<ValidatorResult> ValidateParams(this ValidatorResult validatorResult, IServiceWrapper services, AccountChangePasswordDto dto, int accountId)
        {
            try
            {
                if (dto.Password != dto.ConfirmPassword)
                {
                   // validatorResult.Failures.Add(FAIL_CONFIRM_PASSWORD_MSG);
                    validatorResult.Add(FAIL_CONFIRM_PASSWORD_MSG, nameof(dto.ConfirmPassword));
                }
                var account = await services.Accounts.GetByIdAsync<Account>(accountId);
                if (account == null)
                {
                   // validatorResult.Failures.Add("Tài khoản không tồn tại");
                    validatorResult.Add("Tài khoản không tồn tại", ValidateErrType.NotFound);
                    //return NotFound();
                }
                if (dto.OldPassword != account.Password)
                {
                   // validatorResult.Failures.Add("Nhập mật khẩu cũ thất bại");
                    validatorResult.Add("Nhập mật khẩu cũ thất bại", nameof(dto.Password));
                }
            }
            catch (Exception ex)
            {
                // validatorResult.Failures.Add(ex.ToString());
                validatorResult.Add(ex.ToString());
            }
            return validatorResult;
        }
    }
}