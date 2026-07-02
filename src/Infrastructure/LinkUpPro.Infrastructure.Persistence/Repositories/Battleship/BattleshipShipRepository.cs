using LinkUpPro.Domain.Entities.Battleship;
using LinkUpPro.Domain.Enums.Battleship;
using LinkUpPro.Domain.Interfaces.Repositories.Battleship;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class BattleshipShipRepository
        : GenericRepository<BattleshipShip, Guid>, IBattleshipShipRepository
    {
        public BattleshipShipRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<BattleshipShip>> GetByBoardAsync(Guid boardId)
            => await _dbSet
                   .Where(s => s.BoardId == boardId)
                   .OrderBy(s => s.Size)
                   .ToListAsync();

        public async Task<IReadOnlyList<BattleshipShip>> GetPlacedByBoardAsync(Guid boardId)
            => await _dbSet
                   .Where(s => s.BoardId == boardId && s.StartCoordinateX >= 0 && s.StartCoordinateY >= 0)
                   .ToListAsync();

        // Retorna todas las celdas ocupadas como lista de (row, col)
        public async Task<IReadOnlyList<(int Row, int Col)>> GetOccupiedCellsAsync(
            Guid boardId)
        {
            var ships = await GetPlacedByBoardAsync(boardId);
            var cells = new List<(int, int)>();

            foreach (var ship in ships)
            {
                for (int i = 0; i < (int)ship.Size; i++)
                {
                    var (row, col) = ship.Direction switch
                    {
                        ShipDirection.Right => (ship.StartCoordinateY, ship.StartCoordinateX + i),
                        ShipDirection.Left => (ship.StartCoordinateY, ship.StartCoordinateX - i),
                        ShipDirection.Down => (ship.StartCoordinateY + i, ship.StartCoordinateX),
                        ShipDirection.Up => (ship.StartCoordinateY - i, ship.StartCoordinateX),
                        _ => (ship.StartCoordinateY, ship.StartCoordinateX)
                    };
                    cells.Add((row, col));
                }
            }
            return cells;
        }

        public async Task<bool> AllPlacedAsync(Guid boardId)
        {
            var total = await _dbSet.CountAsync(s => s.BoardId == boardId);
            return total == 5;
        }
    }
}
