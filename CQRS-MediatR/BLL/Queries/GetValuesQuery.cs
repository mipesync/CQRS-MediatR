﻿using CQRS_MediatR.Models;
using MediatR;

namespace CQRS_MediatR.BLL.Queries
{
    public record GetUsersQuery() : IRequest<List<User>>;
    public record GetUserByIdQuery(string id) : IRequest<User>;
}