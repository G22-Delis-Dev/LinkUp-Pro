using LinkUpPro.Domain.Entities.Battleship;
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
    public class BattleshipAttackRepository
        : GenericRepository<BattleshipAttack, Guid>, IBattleshipAttackRepository
    {
        public BattleshipAttackRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<BattleshipAttack>> GetByGameAndAttackerAsync(
            Guid gameId, Guid attackerId)
            => await _dbSet
                   .Include(a => a.Board)
                   .Where(a => a.Board.GameId == gameId && a.Board.PlayerId != attackerId)
                   .OrderBy(a => a.CreatedAt)
                   .ToListAsync();

        public async Task<bool> HasAttackedCellAsync(
            Guid gameId, Guid attackerId, int row, int col)
            => await _dbSet
                   .AnyAsync(a =>
                       a.Board.GameId == gameId &&
                       a.Board.PlayerId != attackerId &&
                       a.CoordinateY == row &&
                       a.CoordinateX == col);

        public async Task<int> CountHitsAsync(Guid gameId, Guid attackerId)
            => await _dbSet
                   .CountAsync(a =>
                       a.Board.GameId == gameId &&
                       a.Board.PlayerId != attackerId &&
                       a.IsHit);

        // Verifica si el atacante hundió todos los barcos del oponente
        public async Task<bool> AllShipsSunkAsync(
            Guid gameId, Guid attackerId, IReadOnlyList<(int Row, int Col)> opponentCells)
        {
            var hits = await _dbSet
                .Where(a => a.Board.GameId == gameId && a.Board.PlayerId != attackerId && a.IsHit)
                .Select(a => new { Row = a.CoordinateY, Col = a.CoordinateX })
                .ToListAsync();

            return opponentCells.All(cell =>
                hits.Any(h => h.Row == cell.Row && h.Col == cell.Col));
        }
    }
}
