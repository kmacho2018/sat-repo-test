using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Sat.Recruitment.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sat.Recruitment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly List<User> _users = new List<User>();

        [HttpPost]
        [Route("/create-user")]
        public async Task<ActionResult<Result>> CreateUser(User user)
        {
            UserValidator validator = new UserValidator();
            ValidationResult results = validator.Validate(user);

            if (!results.IsValid)
            {
                foreach (var failure in results.Errors)
                {
                    Console.WriteLine("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
                }
                return BadRequest(string.Join("", results.Errors));
            }
            else
            {
                decimal percentage = 0;
                if (user.UserType == "Normal")
                {
                    if (user.Money > 100)
                    {
                        percentage = Convert.ToDecimal(0.12);
                    }
                    if ((user.Money < 100) && (user.Money > 10))
                    {
                        percentage = Convert.ToDecimal(0.8);
                    }
                    var gif = user.Money * percentage;
                    user.Money = user.Money + gif;
                }
                if (user.UserType == "SuperUser")
                {
                    if (user.Money > 100)
                    {
                        percentage = Convert.ToDecimal(0.20);
                        var gif = user.Money * percentage;
                        user.Money = user.Money + gif;
                    }
                }
                if (user.UserType == "Premium")
                {
                    if (user.Money > 100)
                    {
                        var gif = user.Money * 2;
                        user.Money = user.Money + gif;
                    }
                }

                var reader = ReadUsersFromFile();

                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLineAsync().Result.Split(',');
                    var user__ = new User
                    {
                        Name = line[0],
                        Email = line[1],
                        Phone = line[2],
                        Address = line[3],
                        UserType = line[4],
                        Money = decimal.Parse(line[5]),
                    };
                    _users.Add(user__);
                }
                reader.Close();
                try
                {
                    var isDuplicated = false;
                    foreach (var user_ in _users)
                    {
                        if (user_.Email == user.Email || user_.Phone == user.Phone)
                        {
                            isDuplicated = true;
                        }
                        else if ((user_.Name == user.Name) && (user_.Address == user.Address))
                        {
                            isDuplicated = true;
                            throw new Exception("User is duplicated");
                        }
                    }

                    if (!isDuplicated)
                    {
                        AddNewUsersToFile(user.Name + "," + user.Email + "," + user.Phone + "," + user.Address + "," + user.UserType + "," + user.Money);

                        Debug.WriteLine("User Created");

                        return new Result()
                        {
                            IsSuccess = true,
                            Errors = "User Created"
                        };
                    }
                    else
                    {
                        Debug.WriteLine("The user is duplicated");

                        return new Result()
                        {
                            IsSuccess = false,
                            Errors = "The user is duplicated"
                        };
                    }
                }
                catch
                {
                    Debug.WriteLine("The user is duplicated");
                    return new Result()
                    {
                        IsSuccess = false,
                        Errors = "The user is duplicated"
                    };
                }
            }
        }

        /// <summary>
        /// Method for get Users From File
        /// </summary>
        /// <returns></returns>
        private StreamReader ReadUsersFromFile()
        {
            var path = Directory.GetCurrentDirectory() + "/Files/Users.txt";

            FileStream fileStream = new FileStream(path, FileMode.Open);

            StreamReader reader = new StreamReader(fileStream);
            return reader;
        }

        /// <summary>
        /// Add new users to File.
        /// </summary>
        /// <param name="userInfo"></param>
        private void AddNewUsersToFile(string userInfo)
        {
            var path = Directory.GetCurrentDirectory() + "/Files/Users.txt";

            using (var fileStream = new FileStream(path, FileMode.Append))
            {
                byte[] bytes = Encoding.UTF8.GetBytes("\n" + userInfo);

                fileStream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}