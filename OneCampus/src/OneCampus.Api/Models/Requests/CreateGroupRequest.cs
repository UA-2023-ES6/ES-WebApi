﻿namespace OneCampus.Api.Models.Requests;

public class CreateGroupRequest
{
    public string Name { get; set; } = null!;

    public int ParentGroupId { get; set; }
}
