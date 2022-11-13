using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WinterProjectAPIV3.Models;

public partial class ShareUser
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    [JsonIgnore]
    public virtual ICollection<UserGroup> UserGroups { get; } = new List<UserGroup>();
}
