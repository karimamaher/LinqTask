using LinqTask.Data;
using LinqTask.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LinqTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationDbContext context= new ApplicationDbContext();

            //1-List all customers' first and last names along with their email addresses
            var customers = context.Customers.AsQueryable().
            Select(e => new {e.FirstName, e.LastName ,e.Email});

            foreach (var item in customers)
            {
                Console.WriteLine($"FirstName: {item.FirstName}, LastName: {item.LastName} ,Email: {item.Email} ");
            }


            //2- Retrieve all orders processed by a specific staff member (e.g., staff_id = 3). 
            var orders = context.Orders.Include(o => o.Staff)
                .Where(o => o.StaffId == 3)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                   StaffName =  o.Staff.FirstName + " "+o.Staff.LastName

                });

            foreach (var item in orders)
            {
                Console.WriteLine($"OrderId: {item.OrderId}, OrderDate: {item.OrderDate} ,StaffName: {item.StaffName} ");
            }


            //3- Get all products that belong to a category named "Mountain Bikes". 

            var products = context.Products.Include(p => p.Category)
                .Where(p => p.Category.CategoryName == "Mountain Bikes")
               .Select(p => new
                  {
                    p.ProductId, 
                    p.ProductName,
                    p.ModelYear,
                    p.ListPrice

               });
            foreach (var item in products)
            {
                Console.WriteLine($"ProductId: {item.ProductId}, ProductName: {item.ProductName} ,ModelYear: {item.ModelYear} ,ListPrice: {item.ListPrice}  ");
                
            }


            //  4 - Count the total number of orders per store.
            var orders2 = context.Orders.Include(o => o.Store)
                 .GroupBy(o => o.StoreId)
                .Select(e => new
                {
                    StoreId = e.Key,
                    OrderCount = e.Count()

                });
            foreach (var item in orders2)
            {
                Console.WriteLine($"StoreId: {item.StoreId}, OrderCount: {item.OrderCount} ");

            }


            // 5 - List all orders that have not been shipped yet(shipped_date is null). 
            var orders3 = context.Orders.Where(o => o.ShippedDate == null);



            //6- Display each customer’s full name and the number of orders they have placed. 
            var result = context.Orders
                .GroupBy(o => new { o.Customer!.FirstName, o.Customer.LastName })
             .Select(e => new
             {
                 CustomerFullName = e.Key.FirstName + " " + e.Key.LastName,
                 orderNumber = e.Count()

             });

            foreach (var item in result)
            {
                Console.WriteLine($":CustomerFullName {item.CustomerFullName}, orderNumber: {item.orderNumber} ");

            }


            // 7 - List all products that have never been ordered(not found in order_items).

            var products1 = context.Products.LeftJoin(
                context.OrderItems,
                p => p.ProductId,
                i => i.ProductId,
                (p, i) => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.ModelYear,
                    p.ListPrice,
                    orderItem = i
                }).Where(e => e.orderItem == null).
                Select(e => new
                {
                    e.ProductId,
                    e.ProductName,
                    e.ModelYear,
                    e.ListPrice
                });


            // 8 - Display products that have a quantity of less than 5 in any store stock.

            var products3 = context.Stocks.Where(s => s.Quantity < 5).
                Select(e => new
                {
                    e.StoreId,
                    e.Product.ProductName,
                    e.Product.ModelYear,
                    e.Product.ListPrice

                });

            //9- Retrieve the first product from the products table. 
            var product = context.Products.FirstOrDefault();


            //10- Retrieve all products from the products table with a certain model year. 

            var products4 = context.Products.Where(p=> p.ModelYear == 2016);


            // 11 - Display each product with the number of times it was ordered.

            var productss = context.OrderItems
            .GroupBy(i => new { i.ProductId, i.Product.ProductName })
            .Select(e => new
              {
                  ProductId = e.Key.ProductId,
                   ProductName = e.Key.ProductName,
                   NumberOfItems = e.Count()
              });   
        

        // 12- Count the number of products in a specific category. 
        var result1 = context.Products.Include(p => p.Category).
                Where(c => c.Category.CategoryName == "Cruisers Bicycles").Count();


            //13- Calculate the average list price of products. 

            var avarage = context.Products.Average(p => p.ListPrice);


            //14- Retrieve a specific product from the products table by ID. 
            var product1 = context.Products.FirstOrDefault(p => p.ProductId == 2);


            // 15 - List all products that were ordered with a quantity greater than 3 in any order.
            var products5 = context.OrderItems.Include(i => i.Product).Where(i => i.Quantity > 3)
                 .Select(e => new
                 {
                     e.ProductId,
                     e.Product.ProductName,
                     e.Product.ListPrice,
                     e.Product.ModelYear
                 });


            //16- Display each staff member’s name and how many orders they processed. 
            var staffs = context.Orders.Include(o => o.Staff)
                .GroupBy(o => new {o.Staff.FirstName , o.Staff.LastName})
                .Select(e => new
                {
                   StaffName =e.Key.FirstName +" "+e.Key.LastName,
                    OrdersNumber = e.Count()
                });


            //17- List active staff members only (active = true) along with their phone numbers. 
            var staffs1 = context.Staffs.Where(s => s.Active == 1)
                .Select(e => new
                {
                    StaffName = e.FirstName + " " + e.LastName,
                    e.Phone
                });


            //18- List all products with their brand name and category name. 
            var products6 = context.Products.Include(p => p.Brand).Include(p => p.Category)
                .Select(e => new
                {
                    e.ProductName,
                    e.ModelYear,
                    e.ListPrice,
                    e.Brand.BrandName,
                    e.Category.CategoryName
                });


            //19- Retrieve orders that are completed = (OrderStatus = 4). 
             var orders5 = context.Orders
             .Where(o => o.OrderStatus == 4);


            //20- List each product with the total quantity sold (sum of quantity from order_items). 

            var product7 = context.OrderItems
                .GroupBy(i => new { i.ProductId ,i.Product.ProductName})
                .Select(e => new
                {
                    ProductId = e.Key.ProductId,
                    ProductName= e.Key.ProductName,
                    TotalQuantity = e.Sum(i => i.Quantity)
                });
        }
    }
}
