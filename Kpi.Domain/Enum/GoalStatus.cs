namespace Kpi.Domain.Enum
{
    public enum GoalStatus
    {
        NoWritte,           // Черновик (создан TeamLeader)
        PendingReview,   // Ожидает проверки CEO
        Returned,        // Возвращен с комментариями CEO
        Approved
    }
}
