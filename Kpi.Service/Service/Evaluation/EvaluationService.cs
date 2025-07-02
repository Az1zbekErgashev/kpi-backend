using DocumentFormat.OpenXml.InkML;
using Kpi.Service.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Kpi.Service.Service.Evaluation
{
    public class EvaluationService
    {
        private readonly IGenericRepository<Domain.Entities.Evaluation> evaluationService;
        private readonly IGenericRepository<Domain.Entities.User.User> userService;

        public EvaluationService(IGenericRepository<Domain.Entities.Evaluation> evaluationService,
            IGenericRepository<Domain.Entities.User.User> userService)
        {
            this.evaluationService = evaluationService;
            this.userService = userService;
        }

        public async ValueTask<bool> CreateOrUpdateAsync(Domain.Entities.Evaluation dto)
        {
            var evaluator = await userService.GetAll(x => x.IsDeleted == 0).Include(u => u.Team).FirstOrDefaultAsync();
            var targetUser = await userService.GetAll(x => x.IsDeleted == 0).Include(u => u.Team).FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (evaluator == null || targetUser == null || evaluator.TeamId != targetUser.TeamId)
                throw new UnauthorizedAccessException("Not authorized");

            var existing = await evaluationService.GetAsync(e => e.UserId == dto.UserId && e.Year == dto.Year && e.Month == dto.Month);

            if (existing != null)
                throw new InvalidOperationException("This user is already evaluated for this month");

            return true;
        }
    }
}
