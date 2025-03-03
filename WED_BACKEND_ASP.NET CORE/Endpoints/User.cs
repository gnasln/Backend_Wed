using WED_BACKEND_ASP.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceStack;
using Wed.Application.Common.Interfaces;
using Wed.Domain.Constants;
using Wed.Domain.Entities;
using WED_BACKEND_ASP.Dtos;
using WED_BACKEND_ASP.Helper.Services;
using WED_BACKEND_ASP.Services;
using NetHelper.Common.Models;
using Microsoft.EntityFrameworkCore;
using WED_BACKEND_ASP.NET_CORE.Infrastructure;

namespace WED_BACKEND_ASP.Endpoints
{
    public class User : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .MapPost(RegisterUser, "/register")
                .MapPost(ForgotPassword, "/forgot-password")
                .MapPost(SendOTP, "/send-otp")
                .MapPost(VerifyOTP, "/verify-otp")
                ;


            app.MapGroup(this)
                .RequireAuthorization()
                .MapPut(ChangePassword, "/change-password")
                .MapPatch(UpdateUser, "/update-user")
                .MapPut(ChangeEmail, "/change-email")
                .MapGet(GetCurrentUser, "/UserInfo")
                .MapPost(CheckPasswork, "/check-password")
                .MapGet(GetUserById, "/get-user/{id}")
                ;

            app.MapGroup(this)
                .RequireAuthorization("admin")
                .MapGet(GetAllUsers, "/get-all-users")
                ;
        }

        public async Task<IResult> RegisterUser([FromBody] RegisterForm newUser,
            UserManager<ApplicationUser> _userManager)
        {
            // Kiểm tra Username
            if (string.IsNullOrEmpty(newUser.UserName))
            {
                return Results.BadRequest("400|Username is required");
            }

            // Kiểm tra Username chỉ chứa chữ cái và chữ số
            if (!newUser.UserName.All(char.IsLetterOrDigit))
            {
                return Results.BadRequest("400|Username can only contain letters or digits.");
            }

            // Kiểm tra xem User đã tồn tại chưa
            var user = await _userManager.FindByNameAsync(newUser.UserName);
            if (user != null)
            {
                return Results.BadRequest("501|User already exists");
            }

            // Kiểm tra mật khẩu
            if (string.IsNullOrEmpty(newUser.Password) && newUser.Password != newUser.RePassword)
            {
                return Results.BadRequest("400|Password is required and must match RePassword");
            }

            // Tạo đối tượng ApplicationUser
            var newUserEntity = new ApplicationUser
            {
                UserName = newUser.UserName,
                Email = newUser.Email,
                FullName = newUser.FullName,
                CellPhone = newUser.CellPhone
            };
            var result = await _userManager.CreateAsync(newUserEntity, newUser.Password!);

            if (result.Succeeded)
            {
                // Gán vai trò cho người dùng mới tạo
                var roleResult = await _userManager.AddToRoleAsync(newUserEntity, Roles.User);

                if (!roleResult.Succeeded)
                {
                    return Results.BadRequest("500|Failed to assign role to user");
                }

                return Results.Ok("200|User created successfully");
            }
            else
            {
                // Trả về lỗi nếu quá trình tạo tài khoản thất bại
                return Results.BadRequest($"500|{string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        public async Task<IResult> ForgotPassword([FromBody] ForgotPassword forgotPassword,
            UserManager<ApplicationUser> userManager, [FromServices] IEmailSender emailSender,
            [FromServices] IBackgroundTaskQueue taskQueue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(forgotPassword.Email))
                {
                    return Results.BadRequest("Email cannot be empty.");
                }

                var user = userManager.Users.FirstOrDefault(u => u.Email == forgotPassword.Email);
                if (user == null)
                {
                    return Results.NotFound("User not found.");
                }

                if(forgotPassword.Password != forgotPassword.ConfirmPassword)
                {
                    return Results.BadRequest("Passwords do not match.");
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, token, forgotPassword.Password!);

                if (result.Succeeded)
                {
                    return Results.Ok("200|Password reset successfully");
                }

                return Results.BadRequest("400|Password reset failed");
            }
            catch (Exception ex)
            {
                return Results.Problem("An error occurred while resetting the password.", statusCode: 500);
            }
        }

        //change password
        public async Task<IResult> ChangePassword(UserManager<ApplicationUser> _userManager,
            [FromBody] ChangePassword changePassword, IUser _user)
        {
            try
            {
                if (changePassword == null || string.IsNullOrEmpty(_user.Id))
                {
                    return Results.BadRequest("400| Missing or invalid user ID or change password data.");
                }

                var currentUser = await _userManager.FindByIdAsync(_user.Id);
                if (currentUser == null)
                {
                    return Results.BadRequest("400| Invalid UserId provided.");
                }

                var isOldPasswordCorrect =
                    await _userManager.CheckPasswordAsync(currentUser, changePassword.oldPassword);
                if (!isOldPasswordCorrect)
                {
                    return Results.BadRequest("400| The old password is incorrect.");
                }

                if (!changePassword.newPassword.Equals(changePassword.comfirmedPassword))
                {
                    return Results.BadRequest("400| The new password and confirmation do not match.");
                }

                var result = await _userManager.ChangePasswordAsync(currentUser, changePassword.oldPassword,
                    changePassword.newPassword);
                if (result.Succeeded)
                {
                    _userManager.UpdateAsync(currentUser).Wait();
                    return Results.Ok("200| Password changed successfully.");
                }
                else
                {
                    var errorDescriptions = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Results.BadRequest($"500| {errorDescriptions}");
                }
            }
            catch (Exception ex)
            {
                // Log the full stack trace here if possible for more in-depth debugging.
                Console.WriteLine($"Error occurred while changing password: {ex}");
                return Results.Problem("An error occurred while changing the password.", statusCode: 500);
            }
        }

/*
C****: Update User
*/
        public async Task<IResult> UpdateUser(UserManager<ApplicationUser> _userManager,
            [FromBody] UpdateUser updateUser,
            [FromServices] IUser _user)
        {
            try
            {
                var userId = _user.Id;
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.BadRequest("400|UserId is empty");
                }

                var currentUser = await _userManager.FindByIdAsync(userId);
                if (currentUser == null)
                {
                    return Results.BadRequest("400|UserId không hợp lệ !");
                }

                if (!updateUser.CellPhone.IsNullOrEmpty()) currentUser.CellPhone = updateUser.CellPhone;
                if (!updateUser.Address.IsNullOrEmpty()) currentUser.Address = updateUser.Address;
                if (!updateUser.FullName.IsNullOrEmpty()) currentUser.FullName = updateUser.FullName;
                currentUser.Gender = updateUser.Gender;
                if (!(updateUser.Birthday == null)) currentUser.Birthday = updateUser.Birthday;

                var result = await _userManager.UpdateAsync(currentUser);
                if (result.Succeeded)
                {
                    return Results.Ok("200|User updated successfully");
                }
                else
                {
                    return Results.BadRequest($"500|{string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error", ex.Message);
                return Results.Problem("An error occurred while updating the user", statusCode: 500);
            }
        }

        //change email
        public async Task<IResult> ChangeEmail([FromBody] ChangeEmail changeEmail,
            [FromServices] UserManager<ApplicationUser> _userManager, IUser _user)
        {
            try
            {
                if (changeEmail == null || string.IsNullOrEmpty(_user.Id))
                {
                    return Results.BadRequest("400| Missing or invalid user ID or change password data.");
                }

                var currentUser = await _userManager.FindByIdAsync(_user.Id);
                if (currentUser == null)
                {
                    return Results.BadRequest("400| Invalid UserId provided.");
                }

                currentUser.Email = changeEmail.NewEmail;
                var res = await _userManager.UpdateAsync(currentUser);
                if (res.Succeeded)
                {
                    return Results.Ok("200| Email changed successfully.");
                }

                return Results.BadRequest("400| Change email failed.");
            }
            catch (Exception ex)
            {
                // Log the full stack trace here if possible for more in-depth debugging.
                Console.WriteLine($"Error occurred while changing password: {ex}");
                return Results.Problem("An error occurred while changing the password.", statusCode: 500);
            }
        }

        public async Task<IResult> SendOTP([FromServices] OTPService _otpService,
            [FromServices] IEmailSender _emailSender, [FromBody] SendOTPRequest request, [FromServices] IUser _user)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                {
                    return Results.BadRequest("400|Email is required");
                }

                var otp = _otpService.GenerateOTP();

                _otpService.SaveOTP(request.Email, otp);

                await _emailSender.SendEmailAsync(request.Email, _user.UserName!
                    , $"Mã xác minh của bạn là: {otp}");

                return Results.Ok("200|OTP sent successfully");
            }
            catch (Exception e)
            {
                return Results.BadRequest("500|Error while sending OTP: " + e.Message);
            }
        }

        public async Task<IResult> VerifyOTP([FromBody] VerifyOTPRequest request, [FromServices] OTPService _otpService,
            [FromServices] UserManager<ApplicationUser> _userManager, [FromServices] IUser _user)
        {
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (currentUser == null)
            {
                return Results.NotFound("khong tim thay nguoi dung.");
            }

            var isValid = false;
            isValid = currentUser.Email != null && _otpService.VerifyOTP(currentUser.Email, request.OTP);

            if (isValid)
            {
                currentUser.EmailConfirmed = true;
                await _userManager.UpdateAsync(currentUser);
                return Results.Ok("200|Xác minh thành công.");
            }

            return Results.BadRequest("Mã xác minh không hợp lệ hoặc đã hết hạn.");
        }

        public async Task<ResultCustomPaginate<IEnumerable<UserDto>>> GetAllUsers(
            [FromServices] UserManager<ApplicationUser> _userManager,
            string? fullName,
            string? email,
            string? cellPhone,
            string? status,
            string? role,
            int page,
            int pageSize)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            // Tìm kiếm theo Fullname
            if (!string.IsNullOrEmpty(fullName))
            {
                usersQuery = usersQuery.Where(u => u.FullName.Contains(fullName));
            }

            // Tìm kiếm theo Email
            if (!string.IsNullOrEmpty(email))
            {
                usersQuery = usersQuery.Where(u => u.Email.Contains(email));
            }

            // Tìm kiếm theo CellPhone
            if (!string.IsNullOrEmpty(cellPhone))
            {
                usersQuery = usersQuery.Where(u => u.CellPhone.Contains(cellPhone));
            }

            // Tìm kiếm theo Status
            if (!string.IsNullOrEmpty(status))
            {
                usersQuery = usersQuery.Where(u => u.Status.Contains(status));
            }

            // Lấy danh sách người dùng và lấy roles ngoài truy vấn để tránh vấn đề với GetRolesAsync
            var usersList = await usersQuery.ToListAsync();

            var usersDtoList = new List<UserDto>();

            foreach (var u in usersList)
            {
                // Lấy roles cho người dùng
                var roles = await _userManager.GetRolesAsync(u);

                var userDto = new UserDto
                {
                    Id = u.Id,
                    Fullname = u.FullName,
                    UserName = u.UserName,
                    Email = u.Email,
                    CellPhone = u.CellPhone,
                    status = u.Status,
                    Birthday = u.Birthday,
                    Address = u.Address,
                    Role = roles.FirstOrDefault(),
                };

                usersDtoList.Add(userDto);
            }

            // Lọc theo Role
            if (!string.IsNullOrEmpty(role))
            {
                usersDtoList = usersDtoList.Where(u => u.Role == role).ToList();
            }

            // Sắp xếp theo CreatedAt
            var sortedUsersDtoList = usersDtoList.OrderByDescending(u => u.Fullname).ToList();

            // Áp dụng phân trang
            var paginatedUsers = sortedUsersDtoList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var result = new ResultCustomPaginate<IEnumerable<UserDto>>
            {
                Data = paginatedUsers,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = sortedUsersDtoList.Count,
                TotalPages = (int)Math.Ceiling((double)sortedUsersDtoList.Count / pageSize)
            };

            return result;
        }


        public async Task<IResult> GetCurrentUser([FromServices] UserManager<ApplicationUser> _userManager, IUser _user)
        {
            var currentUser = await _userManager.FindByIdAsync(_user.Id);
            if (currentUser == null)
            {
                return Results.BadRequest("400|User not found");
            }

            var userDto = new UserDto
            {
                Id = currentUser.Id,
                Fullname = currentUser.FullName,
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Gender = currentUser.Gender,
                CellPhone = currentUser.CellPhone,
                status = currentUser.Status,
                Birthday = currentUser.Birthday,
                Address = currentUser.Address,
                Role = (await _userManager.GetRolesAsync(currentUser)).FirstOrDefault(),
            };

            return Results.Ok(userDto);
        }

        public async Task<IResult> GetUserById([FromServices] UserManager<ApplicationUser> _userManager,
            [FromRoute] string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Results.BadRequest("400|User not found");
                }

                var result = new UserDto
                {
                    Id = user.Id,
                    Fullname = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Gender = user.Gender,
                    CellPhone = user.CellPhone,
                    status = user.Status,
                    Birthday = user.Birthday,
                    Address = user.Address,
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                };
                return Results.Ok(result);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }

        public async Task<IResult> CheckPasswork([FromServices] UserManager<ApplicationUser> _userManager, IUser _user,
            [FromBody] checkPassword request)
        {
            var currentUser = await _userManager.FindByIdAsync(_user.Id);
            if (currentUser != null)
            {
                var isPasswordCorrect = await _userManager.CheckPasswordAsync(currentUser, request.Password);
                if (isPasswordCorrect)
                {
                    return Results.Ok("200|Password is correct");
                }
            }

            return Results.BadRequest("400|Password is incorrect");
        }
    }
}