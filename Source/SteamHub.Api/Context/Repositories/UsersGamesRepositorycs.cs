using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Context;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.UsersGames;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Models.UsersGames;
public class UsersGamesRepository : IUsersGamesRepository
    {
        private readonly DataContext _context;
        public UsersGamesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddToCartAsync(UserGameRequest usersGames)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == usersGames.UserId);
            if (!userExists) throw new Exception("User not found");

            var gameExists = await _context.Games.AnyAsync(g => g.GameId == usersGames.GameId);
            if (!gameExists) throw new Exception("Game not found");


            var userGame = new UsersGames
            {
                UserId = usersGames.UserId,
                GameId = usersGames.GameId,
                IsInCart = true,
                IsPurchased = false,
                IsInWishlist = false
            };

            await _context.UsersGames.AddAsync(userGame);
            await _context.SaveChangesAsync();
        }

        public async Task AddToWishlistAsync(UserGameRequest usersGames)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == usersGames.UserId);
            if (!userExists) throw new Exception("User not found");

            var gameExists = await _context.Games.AnyAsync(g => g.GameId == usersGames.GameId);
            if (!gameExists) throw new Exception("Game not found");

            var userGame = new UsersGames
            {
                UserId = usersGames.UserId,
                GameId = usersGames.GameId,
                IsInCart = false,
                IsPurchased = false,
                IsInWishlist = true
            };

            await _context.UsersGames.AddAsync(userGame);
            await _context.SaveChangesAsync();
        }

        public async Task<GetUserGamesResponse> GetUserCartAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var userGames = await _context.UsersGames
                .Where(ug => ug.UserId == userId && ug.IsInCart)
                .Include(ug => ug.Game)
                .Select(ug => new UserGamesResponse
                {
                    GameId = ug.Game.GameId,
                    IsInCart = ug.IsInCart,
                    IsPurchased = ug.IsPurchased,
                    IsInWishlist = ug.IsInWishlist
                })
                .ToListAsync();

            return new GetUserGamesResponse
            {
                UserGames = userGames
            };
        }

        public async Task<GetUserGamesResponse> GetUserGamesAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var userGames = await _context.UsersGames
                .Where(ug => ug.UserId == userId)
                .Include(ug => ug.Game)
                .Select(ug => new UserGamesResponse
                {
                    GameId = ug.Game.GameId,
                    IsInCart = ug.IsInCart,
                    IsPurchased = ug.IsPurchased,
                    IsInWishlist = ug.IsInWishlist
                })
                .ToListAsync();

            return new GetUserGamesResponse
            {
                UserGames = userGames
            };
        }

        public async Task<GetUserGamesResponse> GetUserPurchasedGamesAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var userGames = await _context.UsersGames
                .Where(ug => ug.UserId == userId && ug.IsPurchased)
                .Include(ug => ug.Game)
                .Select(ug => new UserGamesResponse
                {
                    GameId = ug.Game.GameId,
                    IsInCart = ug.IsInCart,
                    IsPurchased = ug.IsPurchased,
                    IsInWishlist = ug.IsInWishlist

                })
                .ToListAsync();

            return new GetUserGamesResponse
            {
                UserGames = userGames
            };
        }

        public async Task<GetUserGamesResponse> GetUserWishlistAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var userGames = await _context.UsersGames
                .Where(ug => ug.UserId == userId && ug.IsInWishlist)
                .Include(ug => ug.Game)
                .Select(ug => new UserGamesResponse
                {
                    GameId = ug.Game.GameId,
                    IsInCart = ug.IsInCart,
                    IsPurchased = ug.IsPurchased,
                    IsInWishlist = ug.IsInWishlist
                })
                .ToListAsync();

            return new GetUserGamesResponse
            {
                UserGames = userGames
            };
        }

        public async Task PurchaseGameAsync(UserGameRequest usersGames)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == usersGames.UserId);
            if (!userExists) throw new Exception("User not found");

            var gameExists = await _context.Games.AnyAsync(g => g.GameId == usersGames.GameId);
            if (!gameExists) throw new Exception("Game not found");

            var userGame = await _context.UsersGames
                .FirstOrDefaultAsync(ug => ug.UserId == usersGames.UserId && ug.GameId == usersGames.GameId);

            if (userGame == null)
            {
                userGame = new UsersGames
                {
                    UserId = usersGames.UserId,
                    GameId = usersGames.GameId,
                    IsInCart = false,
                    IsPurchased = true,
                    IsInWishlist = false
                };
                await _context.UsersGames.AddAsync(userGame);
                await _context.SaveChangesAsync();
            }
            else
            {
                userGame.IsInCart = false;
                userGame.IsPurchased = true;
                userGame.IsInWishlist = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(UserGameRequest usersGames)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == usersGames.UserId);
            if (!userExists) throw new Exception("User not found");

            var gameExists = await _context.Games.AnyAsync(g => g.GameId == usersGames.GameId);
            if (!gameExists) throw new Exception("Game not found");

            var userGame = await _context.UsersGames
                .FirstOrDefaultAsync(ug => ug.UserId == usersGames.UserId && ug.GameId == usersGames.GameId);
            if (userGame != null && userGame.IsInCart)
            {
                userGame.IsInCart = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Game not found in cart");
            }
        }

        public async Task RemoveFromWishlistAsync(UserGameRequest usersGames)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == usersGames.UserId);
            if (!userExists) throw new Exception("User not found");

            var gameExists = await _context.Games.AnyAsync(g => g.GameId == usersGames.GameId);
            if (!gameExists) throw new Exception("Game not found");

            var userGame = _context.UsersGames
                .FirstOrDefault(ug => ug.UserId == usersGames.UserId && ug.GameId == usersGames.GameId);
            if (userGame != null && userGame.IsInWishlist)
            {
                userGame.IsInWishlist = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Game not found in wishlist");
            }
        }
    }
