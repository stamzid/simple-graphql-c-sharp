// See https://aka.ms/new-console-template for more information
using Npgsql;
using System;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using StackExchange.Redis;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace api
{
    public class PostgresConnector {
        private static string Host = "notifi_postgres";
        private static string User = "notifi";
        private static string DBname = "notifi";
        private static string Password = "notifi";
        private static string Port = "5432";
        public static int insert() {
            string connString =
                String.Format(
                    "Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                    Host,
                    User,
                    DBname,
                    Port,
                    Password);
            var utcNow = DateTime.UtcNow;
            try {
                using (var conn = new NpgsqlConnection(connString)) {
                    Console.Out.WriteLine("Opening connection");
                    conn.Open();
                    using (var command = new NpgsqlCommand("INSERT INTO api_hits (time) VALUES (@utcNow)", conn))
                        {
                            command.Parameters.AddWithValue("utcNow", utcNow);
                            return command.ExecuteNonQuery();
                        }
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return 0;
            }
        }
        public static int count_rows() {
            string connString =
                String.Format(
                    "Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                    Host,
                    User,
                    DBname,
                    Port,
                    Password);
            try {
                using (var conn = new NpgsqlConnection(connString)) {
                    Console.Out.WriteLine("Opening connection");
                    conn.Open();
                    using (var command = new NpgsqlCommand("SELECT count(*) FROM notifi.api_hits", conn))
                        {
                            var rows = 0;
                            var reader = command.ExecuteReader();
                            if (reader.Read()){
                                rows = reader.GetInt32(0);
                            }
                            reader.Close();
                            return rows;
                        }
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return 0;
            }
        }

    }

    public class RedisConnector {
        public static async Task<bool> CheckRedisConnection() {
            try {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
                    new ConfigurationOptions{
                        EndPoints = { "notifi_redis:6379" },                
                    });
                var db = redis.GetDatabase();
                var pong = await db.PingAsync();
                Console.WriteLine(pong);
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
    public class TokenGenerator {
        public static string GenerateToken() {
            var someClaims = new Claim[]{
                new Claim(JwtRegisteredClaimNames.UniqueName,"RANDOM_USER"),
                new Claim(JwtRegisteredClaimNames.NameId,Guid.NewGuid().ToString())
            };

            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("RANDOM_TEST_KEY_FOR_JWT"));
            var token = new JwtSecurityToken(
                issuer: "notify.test",
                audience: "notify.public",
                claims: someClaims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RedisConnection {
        public bool Active() {
            PostgresConnector.insert();
            var redisTask = RedisConnector.CheckRedisConnection();
            redisTask.Wait();
            return redisTask.Result;
        }
    }
    public class TotalApiHits {
        public int Hits(){
            PostgresConnector.insert();
            return PostgresConnector.count_rows();
        }
    }
    public class Greeting {
        public string Message() {
            PostgresConnector.insert();
            return "Hello World";
        } 
    } 

    public class AuthToken {
        public string Token () {
            PostgresConnector.insert();
            var token = TokenGenerator.GenerateToken();
            return token; 
        }
    }
    public class Query {
        public Greeting GetGreeting() => new Greeting();
        public AuthToken GetAuthToken() => new AuthToken();
        public TotalApiHits GetTotalApiHits() => new TotalApiHits();
        public RedisConnection GetRedisConnection() => new RedisConnection();
    }

    class api {
        static void Main(string[] args) {

            Console.WriteLine("Hello World!");
            WebHost
                .CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                    services
                        .AddGraphQLServer()
                        .AddQueryType<Query>())
                .Configure(builder =>
                    builder
                        .UseRouting()
                        .UseEndpoints(e => e.MapGraphQL()))
                .Build()
                .Run();

        }
    }
}