﻿namespace Wonka.Models;

public class Customer
{
    public int? Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string EmailAddress { get; set; } = string.Empty;

    public int Age { get; set; }
}
