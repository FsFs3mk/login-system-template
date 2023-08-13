using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using BCrypt.Net;

public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
}

public class Program
{
    private static List<User> users = new List<User>();

    public static void Main()
    {
        LoadUsersFromJsonFile();

        Console.WriteLine("Welcome to the Login System!");

        while (true)
        {
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterUser();
                    SaveUsersToJsonFile();
                    break;
                case "2":
                    Login();
                    break;
                case "3":
                    Console.WriteLine("Exiting the Login System. Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            Console.WriteLine();
        }
    }

    private static void LoadUsersFromJsonFile()
    {
        string filePath = "users.json";

        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            users = JsonConvert.DeserializeObject<List<User>>(jsonString);
        }
    }

    private static void SaveUsersToJsonFile()
    {
        string filePath = "users.json";
        string jsonString = JsonConvert.SerializeObject(users, Formatting.Indented);
        File.WriteAllText(filePath, jsonString);
    }

    private static void RegisterUser()
    {
        Console.Write("Enter Username: ");
        string username = Console.ReadLine();

        Console.Write("Enter Email: ");
        string email = Console.ReadLine();

        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        User newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash
        };

        users.Add(newUser);

        Console.WriteLine("Registration successful!");
    }

    private static void Login()
    {
        Console.Write("Enter Username/Email: ");
        string usernameOrEmail = Console.ReadLine();

        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        User user = users.Find(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

        if (user != null)
        {
            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                Console.WriteLine("Login successful");
                Console.WriteLine("Welcome, " + user.Username + "!");
                Console.WriteLine("Your email is: " + user.Email);
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