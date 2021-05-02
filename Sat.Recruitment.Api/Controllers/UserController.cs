using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Sat.Recruitment.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                if (user.UserType == "Normal")
                {
                    if (user.Money > 100)
                    {
                        var percentage = Convert.ToDecimal(0.12);
                        //If new user is normal and has more than USD100
                        var gif = user.Money * percentage;
                        user.Money = user.Money + gif;
                    }
                    if (user.Money < 100)
                    {
                        if (user.Money > 10)
                        {
                            var percentage = Convert.ToDecimal(0.8);
                            var gif = user.Money * percentage;
                            user.Money = user.Money + gif;
                        }
                    }
                }
                if (user.UserType == "SuperUser")
                {
                    if (user.Money > 100)
                    {
                        var percentage = Convert.ToDecimal(0.20);
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

                //Normalize email
                var aux = user.Email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

                aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

                user.Email = string.Join("@", new string[] { aux[0], aux[1] });

                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLineAsync().Result;

                    _users.Add(user);
                }
                reader.Close();
                try
                {
                    var isDuplicated = false;
                    foreach (var user_ in _users)
                    {
                        if (user_.Email == user.Email
                            ||
                            user_.Phone == user.Phone)
                        {
                            isDuplicated = true;
                        }
                        else if (user_.Name == user.Name)
                        {
                            if (user_.Address == user.Address)
                            {
                                isDuplicated = true;
                                throw new Exception("User is duplicated");
                            }
                        }
                    }

                    if (!isDuplicated)
                    {
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

                return new Result()
                {
                    IsSuccess = true,
                    Errors = "User Created"
                };
            }
        }

        private StreamReader ReadUsersFromFile()
        {
            var path = Directory.GetCurrentDirectory() + "/Files/Users.txt";

            FileStream fileStream = new FileStream(path, FileMode.Open);

            StreamReader reader = new StreamReader(fileStream);
            return reader;
        }
    }
}