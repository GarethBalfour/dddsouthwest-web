﻿using System.Threading.Tasks;
using Dapper;
using MediatR;
using Npgsql;

namespace DDDSouthWest.Domain.Features.Account.ManagePages.ViewPageDetail
{
    public class ViewPageDetail
    {
        public class Query : IRequest<PageDetailModel>
        {
            public int Id { get; set; }
        }

        public class Handler : IAsyncRequestHandler<Query, PageDetailModel>
        {
            private readonly ClientConfigurationOptions _options;

            public Handler(ClientConfigurationOptions options)
            {
                _options = options;
            }

            public async Task<PageDetailModel> Handle(Query message)
            {
                using (var connection = new NpgsqlConnection(_options.Database.ConnectionString))
                {
                    var response = await connection.QuerySingleOrDefaultAsync<PageDetailModel>(
                        "SELECT Id, Title, Filename, BodyMarkdown, IsLive FROM pages WHERE Id = @Id LIMIT 1",
                        new
                        {
                            Id = message.Id
                        });

                    return response;
                }                
            }
        }
    }
}