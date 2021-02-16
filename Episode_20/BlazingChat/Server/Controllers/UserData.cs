using System.Collections.Generic;
using System.Linq;
using BlazingChat.Server.Models;

public class UserData
{
    public List<User> Users = new List<User>();
    
    public List<User> GetAllUsers()
    {
        var results = Enumerable.Range(0, 15000);
        Users.AddRange(results.Select(x => new User { UserId = x, FirstName = $"First{x}", LastName = $"Last{x}"}));
        
        return Users;
    }
    public List<User> GetSomeUsers(int startIndex, int numberOfUsers)
    {
        var results = Enumerable.Range(startIndex, numberOfUsers);
        Users.AddRange(results.Select(x => new User { UserId = x, FirstName = $"First{x}", LastName = $"Last{x}"}));

        return Users;
    }
}

