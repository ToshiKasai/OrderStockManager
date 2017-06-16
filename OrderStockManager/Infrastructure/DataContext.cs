using OrderStockManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace OrderStockManager.Infrastructure
{
    public class DataContext: DbContext
    {
        // private const string CacheKey = "__DataContext__";

        public DataContext()
            : base("DefaultConnection")
        {
            // Configuration.ProxyCreationEnabled = false;
            // Configuration.LazyLoadingEnabled = false;
        }

        public static DataContext Create()
        {
            return new DataContext();
        }

        /*
        public static bool HasContext
        {
            get { return HttpContext.Current.Items[CacheKey] != null; }
        }
        */

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                this.Database.Log = (log) => System.Diagnostics.Debug.WriteLine(log);
                return 0;
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public DbSet<UserModel> UserModels { get; set; }
        public DbSet<RoleModel> RoleModels { get; set; }
        public DbSet<UserRoleModel> UserRoleModels { get; set; }

        public DbSet<MakerModel> MakerModels { get; set; }
    }
}
