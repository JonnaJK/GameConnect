﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameConnect.Contracts.Requests;
public class CreateTagRequest
{
    public required string Name { get; init; }
}
