using Dapper;
using DapperApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DapperApp.Repositories
{
    public class ItemsRepository
    {
        private readonly string _connectionString;

        public ItemsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(string Name)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn
                    .Query<int>(
                        @"insert into dbo.Items(Name) values (@name);
                          select cast(scope_identity() as int);",
                        new { name = Name })
                    .First();
            }
        }

        public List<Item> Select()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn
                    .Query<Item>(@"select Id, Name from dbo.Items;")
                    .ToList();
            }
        }

        public Item Select(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn
                    .Query<Item>(
                        @"select Id, Name from dbo.Items where Id = @id;",
                        new { id })
                    .Single();
            }
        }

        public Item Select(string name)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn
                    .Query<Item>(
                        @"select Id, Name from dbo.Items where Name = @name;",
                        new { name })
                    .Single();
            }
        }

        public void Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Execute(
                    @"delete from dbo.Items where Id = @id;",
                    new { id });
            }
        }

        public void Update(int id, string name)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Execute(
                    @"update dbo.Items set Name = @name where Id = @id;",
                    new { id, name });
            }
        }
    }
}
