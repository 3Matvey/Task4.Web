using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Task4.Web.Data
{
    internal static class UserDbErrors
    {
        private const string DuplicateEmailConstraintName = "IX_users_Email";

        public static bool IsDuplicateEmail(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException postgresException
                && postgresException.SqlState == PostgresErrorCodes.UniqueViolation
                && postgresException.ConstraintName == DuplicateEmailConstraintName;
        }
    }
}
