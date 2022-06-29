using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NorthwindDb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NorthwindLinq
{
  public class Queries
  {
    private NorthwindContext db;
    public Queries()
    {
      InitializeDatabase();
    }

    private void InitializeDatabase()
    {
      var config = new ConfigurationBuilder()
         .SetBasePath(AppContext.BaseDirectory)
         .AddJsonFile("appsettings.json")
         .Build();
      var connectionString = config.GetConnectionString("NorthwindDb");
      var options = new DbContextOptionsBuilder<NorthwindContext>()
                        //.LogTo(Console.WriteLine, LogLevel.Information)
                        .UseSqlServer(connectionString)
                        .Options;
      db = new NorthwindContext(options);
    }

    public void CheckAll()
    {
      First5ProductsWithCategories().Show("First5ProductsWithCategories");
      OrderedProductsOfBOTTM().Show("OrderedProductsOfBOTTM");
      NrOfEmployeesWhoSoldToCustomersInGivenCity("Nantes").Show("NrOfEmployeesWhoSoldToCustomersInGivenCity Nantes");
      NrOfEmployeesWhoSoldToCustomersInGivenCity("London").Show("NrOfEmployeesWhoSoldToCustomersInGivenCity London");
      NrOfEmployeesWhoSoldToCustomersInGivenCity("Buenos Aires").Show("NrOfEmployeesWhoSoldToCustomersInGivenCity Buenos Aires");
      CustomersWithUnshippedOrders().Show("CustomersWithUnshippedOrders");
      TotalQuantityOfShipper("Speedy Express").Show("TotalQuantityOfShipper Speedy Express");
      TotalQuantityOfShipper("United Package").Show("TotalQuantityOfShipper United Package");
      TotalQuantityOfShipper("Federal Shipping").Show("TotalQuantityOfShipper Federal Shipping");
      AveragePriceOfSuppliersOfCity("Tokyo").Show("AveragePriceOfSuppliersOfCity Tokyo");
      AveragePriceOfSuppliersOfCity("Paris").Show("AveragePriceOfSuppliersOfCity Paris");
      AveragePriceOfSuppliersOfCity("Berlin").Show("AveragePriceOfSuppliersOfCity Berlin");
      CategoriesWithProductsInStockMoreThan(400).Show("CategoriesWithProductsInStockMoreThan 400");
      CategoriesWithProductsInStockMoreThan(600).Show("CategoriesWithProductsInStockMoreThan 600");
      CategoriesWithProductsInStockMoreThan(200).Show("CategoriesWithProductsInStockMoreThan 200");
    }

    #region Q01
    public List<string> First5ProductsWithCategories()
    {
      try
      {
        return db.Products
          .OrderBy(x => x.ProductName)
          .Select(x => new { x.ProductName, x.Category.CategoryName })
          .Take(5)
          .ToList() //to be able to use interpolated strings
          .Select(x => $"{x.ProductName} - {x.CategoryName}")
          .ToList();
      }
      catch (Exception exc)
      {
        Console.WriteLine(exc.ToString());
      }
      return new List<string>();
    }
    #endregion

    #region Q02
    public List<string> OrderedProductsOfBOTTM()
    {
      return db.OrderDetails
        .Where(x => x.Order.CustomerId == "BOTTM" && x.Product.UnitPrice > 30)
        .Select(x => x.Product.ProductName)
        .Distinct()
        .OrderBy(x => x).ToList();
    }
    #endregion

    #region Q03
    public int NrOfEmployeesWhoSoldToCustomersInGivenCity(string city)
    {
      return db.Orders
         .Where(x => x.Customer.City == city)
         .Select(x => x.Employee.LastName)
         .Distinct()
         .Count();
    }
    #endregion

    #region Q04
    public List<string> CustomersWithUnshippedOrders()
    {
      return db.Orders
        .Include(x => x.Customer)
        .Include(x => x.Employee)
        .Where(x => x.OrderDate != null && x.ShippedDate == null
              && (x.Customer.Country == "Venezuela" || x.Customer.Country == "Argentina"))
        .OrderBy(x => x.Customer.CompanyName)
        .ThenBy(x => x.Employee.LastName)
        .ToList()
        .Select(x => $"{x.Customer.CompanyName} - {x.Customer.City}/{x.Customer.Country} - {x.Employee.FirstName} {x.Employee.LastName}")
        .Distinct()
        .ToList();
    }
    #endregion

    #region Q05
    public int TotalQuantityOfShipper(string shipperCompany)
    {
      return db.OrderDetails
        .Where(x => x.Order.ShipViaNavigation.CompanyName == shipperCompany && x.Order.ShippedDate != null)
        .Select(x => (int)x.Quantity)
        .Sum();
    }
    #endregion

    #region Q06
    public double AveragePriceOfSuppliersOfCity(string city)
    {
      return db.Products
        .Where(x => x.Supplier.City == city)
        .Select(x => (double)x.UnitPrice)
        .Average();
    }
    #endregion

    #region Q07
    public List<string> CategoriesWithProductsInStockMoreThan(int totalStock)
    {
      //var q_ = from p in db.Products
      //        join c in db.Categories
      //        on p.CategoryID equals c.CategoryID
      //        group (int)p.UnitsInStock by c.CategoryName;
      var q = db.Products
        .Include(x => x.Category)
        .AsEnumerable()
        .GroupBy(x => x.Category.CategoryName, x => (int)x.UnitsInStock);
      var st = q.ToDictionary(x => x.Key, x => x.Sum());
      return st.Where(x => x.Value > totalStock)
               .Select(x => x.Key)
               .OrderBy(x => x)
               .ToList();
    }
    #endregion

  }
}
