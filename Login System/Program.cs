using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using BCrypt.Net;

namespace UserRegistrationAndLogin
{
    public class User
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.Write("Enter your choice: ");

                int choice;
                string Result = Console.ReadLine();

                while (!Int32.TryParse(Result, out choice))
                {
                    Console.WriteLine("Not a valid number, try again.");
                    Result = Console.ReadLine();
                }

                if (choice == 1)
                {
                    Register();
                }
                else if (choice == 2)
                {
                    Login();
                }
                else
                {
                    Console.WriteLine("Invalid choice, try again.");
                }
            }
        }

        static void Register()
        {
            Console.Write("Enter Email: ");
            string email = Console.ReadLine();

            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Username cannot be empty");
                return;
            }

            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Password cannot be empty");
                return;
            }

            var usersList = new List<User>();

            if (File.Exists("users.json"))
            {
                var jsonData = File.ReadAllText("users.json");
                usersList = JsonConvert.DeserializeObject<List<User>>(jsonData);
            }

            if (usersList.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already exists");
                return;
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            usersList.Add(new User { Email = email, Username = username, Password = passwordHash });

            var convertedJson = JsonConvert.SerializeObject(usersList, Formatting.Indented);
            File.WriteAllText("users.json", convertedJson);

            Console.WriteLine("User registered successfully!");
        }

        public static void Login()
        {
            Console.Write("Enter Username/Email: ");
            string usernameOrEmail = Console.ReadLine();

            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            if (File.Exists("users.json"))
            {
                var jsonData = File.ReadAllText("users.json");
                var usersList = JsonConvert.DeserializeObject<List<User>>(jsonData);

                var user = usersList.FirstOrDefault(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

                if (user != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                    {
                        Console.WriteLine("Login successful");
                        Console.WriteLine("Welcome, " + user.Username + "!");
                        Console.WriteLine("Your email is: " + user.Email);
                        // Add more code here to do other things after the user logs in
                    }
                    else
                    {
                        Console.WriteLine("Invalid Password");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Username/Email");
                }
            }
        }
    }
}