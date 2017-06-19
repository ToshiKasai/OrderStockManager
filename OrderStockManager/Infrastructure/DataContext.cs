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
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
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

        public DbSet<ContainerModel> ContainerModels { get; set; }
        public DbSet<GroupModel> GroupModels { get; set; }
        public DbSet<GroupProductModel> GroupProductModels { get; set; }
        public DbSet<MakerModel> MakerModels { get; set; }
        public DbSet<OfficeModel> OfficeModels { get; set; }
        public DbSet<ProductModel> ProductModels { get; set; }
        public DbSet<SalesModel> SalesModels { get; set; }
        public DbSet<SalesTrendModel> SalesTrendModels { get; set; }
        public DbSet<StockModel> StockModels { get; set; }
        public DbSet<TradeModel> TradeModels { get; set; }
        public DbSet<UserMakerModel> UserMakerModels { get; set; }

        public DbSet<SignInLogModel> SignInLogModels { get; set; }
        public DbSet<ApplicationLogModel> ApplicationLogModel { get; set; }

        public DbSet<CurrentStockModel> CurrentStockModels { get; set; }
        public DbSet<InvoiceModel> InvoiceModels { get; set; }
        public DbSet<OrderModel> OrderModels { get; set; }
    }
}
