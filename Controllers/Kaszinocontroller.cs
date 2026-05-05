using Kaszinó_projekt.Modells;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Kaszinó_projekt.Controllers
{
    [ApiController]
    public class Kaszinocontroller : ControllerBase
    {
        private readonly AppContext appContext;
        private readonly IConfiguration configuration;

        public Kaszinocontroller(AppContext appContext, IConfiguration configuration)
        {
            this.appContext = appContext;
            this.configuration = configuration;
        }

        [HttpGet("/api/felhasznalok")]
        public IActionResult GetFelhasznalok()
        {
            var felhasznalok = appContext.Set<Modells.Adatokmodell>()
                .Select(a => new
                {
                    a.Id,
                    a.Nev,
                    a.Email,
                    a.Jelszo,
                    a.Eletkor,
                    a.Egyenleg,
                    a.Admin
                })
                .ToList();
            return Ok(felhasznalok);
        }

        [HttpPost("/api/felhasznalok/regisztracio")]
        public IActionResult PutRegisztracio([FromBody] Modells.Adatokmodell ujAdat)
        {
            appContext.Set<Modells.Adatokmodell>().Add(ujAdat);
            appContext.SaveChanges();
            return Ok();
        }

        [HttpPost("/api/auth/login")]
        public IActionResult Login([FromBody] Modells.LoginRequest req)
        {
            var user = appContext.Set<Modells.Adatokmodell>()
                .FirstOrDefault(x =>
                    (x.Nev == req.NevVagyEmail || x.Email == req.NevVagyEmail) &&
                    x.Jelszo == req.Jelszo);

            if (user == null)
                return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("admin", user.Admin ? "1" : "0")
            };

            var jwtKey = configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT key not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpGet("/api/felhasznalok/{id}")]
        [Authorize]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var user = appContext.Set<Adatokmodell>()
                    .Where(u => u.Id == id)
                    .Select(u => new
                    {
                        u.Id,
                        u.Nev,
                        u.Email,
                        u.Eletkor,
                        u.Egyenleg,
                        u.Admin
                    })
                    .FirstOrDefault();

                if (user == null)
                    return NotFound("Felhasználó nem található");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hiba történt: {ex.Message}");
            }
        }

        [HttpPut("/api/felhasznalok/{id}/egyenleg")]
        [Authorize]
        public IActionResult UpdateBalance(int id, [FromBody] BalanceUpdateRequest request)
        {
            try
            {
                var user = appContext.Set<Adatokmodell>().Find(id);
                if (user == null)
                    return NotFound("Felhasználó nem található");

                var currentUserId = User.FindFirst("id")?.Value;
                var isAdmin = User.FindFirst("admin")?.Value == "1";

                if (currentUserId != id.ToString() && !isAdmin)
                    return Unauthorized("Nincs jogosultság az egyenleg módosításához");

                if (request.Osszeg < 0 && user.Egyenleg + request.Osszeg < 0)
                    return BadRequest("Nincs elegendő egyenleg");

                user.Egyenleg += request.Osszeg;
                appContext.SaveChanges();

                return Ok(new { egyenleg = user.Egyenleg });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hiba történt: {ex.Message}");
            }
        }

        [HttpGet("users")]
        [Authorize]
        public IActionResult GetUsers()
        {
            if (User.FindFirst("admin")?.Value != "1")
                return Unauthorized();

            return Ok(appContext.Set<Adatokmodell>().ToList());
        }

        [HttpDelete("/api/felhasznalok/{id}")]
        [Authorize]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                if (User.FindFirst("admin")?.Value != "1")
                    return Unauthorized("Csak admin törölhet felhasználót");

                var user = appContext.Set<Adatokmodell>().Find(id);
                if (user == null)
                    return NotFound("Felhasználó nem található");

                appContext.Set<Adatokmodell>().Remove(user);
                appContext.SaveChanges();

                return Ok(new { message = "Felhasználó sikeresen törölve" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hiba történt: {ex.Message}");
            }
        }

        // ========== JÁTÉK MENTÉSE - JAVÍTVA ==========
        [HttpPost("/api/jatekok/mentes")]
        [Authorize]
        public IActionResult SaveGame([FromBody] JátékokAdatai gameData)
        {
            try
            {
                Console.WriteLine($"📥 Játék mentése érkezett: FelhasznaloId={gameData.FelhasznaloId}, Tet={gameData.Tet}, Nyeremeny={gameData.Nyeremeny}");

                var currentUserId = User.FindFirst("id")?.Value;
                if (currentUserId != gameData.FelhasznaloId.ToString())
                    return Unauthorized("Csak saját játékot menthetsz");

                gameData.Datum = DateTime.Now;
                gameData.Felhasznalo = null; // Ne várja el a navigációs tulajdonságot

                appContext.Set<JátékokAdatai>().Add(gameData);
                appContext.SaveChanges();

                Console.WriteLine($"✅ Játék sikeresen elmentve! ID: {gameData.JátékId}");

                return Ok(new { success = true, gameId = gameData.JátékId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Hiba a játék mentésekor: {ex.Message}");
                return StatusCode(500, $"Hiba: {ex.Message}");
            }
        }

        // ========== STATISZTIKÁK LEKÉRÉSE ==========
        [HttpGet("/api/felhasznalok/{id}/statisztikak")]
        [Authorize]
        public IActionResult GetUserStats(int id)
        {
            try
            {
                Console.WriteLine($"📊 Statisztikák lekérése: FelhasznaloId={id}");

                var games = appContext.Set<JátékokAdatai>()
                    .Where(j => j.FelhasznaloId == id)
                    .ToList();

                Console.WriteLine($"📊 Talált játékok száma: {games.Count}");

                if (!games.Any())
                {
                    return Ok(new
                    {
                        totalWon = 0,
                        totalBet = 0,
                        gamesPlayed = 0,
                        winRate = 0.0,
                        netProfit = 0,
                        biggestWin = 0,
                        favoriteGame = "-",
                        recentGames = new List<object>()
                    });
                }

                var totalBet = games.Sum(g => g.Tet);
                var totalWon = games.Sum(g => g.Nyeremeny);
                var gamesPlayed = games.Count;
                var biggestWin = games.Max(g => g.Nyeremeny);
                var netProfit = totalWon - totalBet;
                var winRate = totalBet > 0 ? (double)totalWon / totalBet * 100 : 0;

                var favoriteGame = games
                    .GroupBy(g => g.JatekTipus)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault() ?? "-";

                var recentGames = games
                    .OrderByDescending(g => g.Datum)
                    .Take(5)
                    .Select(g => new
                    {
                        gameType = g.JatekTipus,
                        result = g.Nyeremeny - g.Tet,
                        date = g.Datum
                    })
                    .ToList();

                var result = new
                {
                    totalWon,
                    totalBet,
                    gamesPlayed,
                    winRate = Math.Round(winRate, 1),
                    netProfit,
                    biggestWin,
                    favoriteGame,
                    recentGames
                };

                Console.WriteLine($"📊 Statisztikák sikeresen lekérve: totalWon={totalWon}, totalBet={totalBet}, gamesPlayed={gamesPlayed}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Hiba a statisztikák lekérésekor: {ex.Message}");
                return StatusCode(500, $"Hiba: {ex.Message}");
            }
        }
    }

    // BalanceUpdateRequest osztály
    public class BalanceUpdateRequest
    {
        public int Osszeg { get; set; }
    }
}