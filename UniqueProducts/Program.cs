using UniqueProducts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections;


class Program
{
    static void Main(string[] args)
    {
        using UniqueProductsContext ctx = new();
        //Выполняем разные методы, содержащие операции выборки и изменения данных
        Console.WriteLine("==== Будут выполнены выборки данных (нажмите любую клавишу) ====");
        Console.ReadKey();
        Select(ctx);

        Console.WriteLine("==== Будут выполнены вставки записей (нажмите любую клавишу) ====\n");
        Console.ReadKey();
        Insert(ctx);

        Console.WriteLine("==== Будет выполнено удаление данных (нажмите любую клавишу) ====");
        Console.ReadKey();
        Delete(ctx);

        Console.WriteLine("====== Будет выполнено обновление данных (нажмите любую клавишу) ========");
        Console.ReadKey();
        Update(ctx);
    }

    static void Print(string sqltext, IEnumerable items)
    {
        Console.WriteLine(sqltext);
        Console.WriteLine("Записи: ");
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
        }
        Console.WriteLine("\nНажмите любую клавишу\n");
        Console.ReadKey();
    }

    //Выборки данных | Задания 1-5
    static void Select(UniqueProductsContext db)
    {
        //1. Выборка всех записей из таблицы Product (Для наглядности будут выбраны 5 записей)
        var products = from product in db.Products
            select new
            {
                //Для удобства будем выводить значения только некоторых полей
                Код_изделия = product.ProductId,
                Название_изделия = product.ProductName,
                Вес_изделия = product.ProductWeight,
                Цена_изделия = product.ProductPrice
            };
        string comment = "1. Результат выполнения запроса на выборку записей из таблицы Products(изделия) : \r\n";
        Print(comment, products.Take(5).ToList());


        //2. Выборка изделий с ценой меньше заданной (Для наглядности будут выбраны 5 записей), используя методы расширений
        var productsWithPrice = db.Products.Where(o => (o.ProductPrice<15))
           .OrderBy(o => o.ProductId)
           .Select(gr => new
                {
                    Код_изделия = gr.ProductId,
                    Название_изделия= gr.ProductName,
                    Цена_изделия = gr.ProductPrice
                }
           );

        comment = "2. Результат выполнения запроса на выборку записей с ценой < 15 из таблицы Products : \r\n";
        Print(comment, productsWithPrice.Take(5).ToList());

        //3. Выборка данных из таблицы изделия, сгруппированных по коду материала, с выводом макисмальной цены
        var productByPrice = from o in db.Products
                             group o.ProductPrice by o.MaterialId into gr
                             select new
                             {
                                 Код_материала=gr.Key,
                                 Цена_изделия = gr.Max()
                             };
        comment = "3. Результат выполнения запроса на выборку данных из таблицы Products,\n записи сгруппированы по коду материала (MaterialId) с выводом макисмальной цены : \r\n";
        Print(comment, productByPrice.Take(10).ToList());

        //4. Выборка данных из таблиц изделия и материалы, с использованием методов расширения
        var products_materials = db.Products.OrderBy(p => p.ProductId)
            .Join(db.Materials, p => p.ProductId, m => m.MaterialId, (p, m) => new { p.ProductName,m.MaterialName });
        comment = "4. Результат выполнения запроса на выборку данных из таблиц Products и Materials: \r\n";
        Print(comment, products_materials.Take(5).ToList());

        //5. Выборка данных из изделия и материалы, с фильтрацией по цене
        var products_materialsWithPrice = db.Products.OrderBy(p => p.ProductId)
            .Where(p=>p.ProductPrice<10)
            .Join(db.Materials, p => p.ProductId, m => m.MaterialId, (p, m) => new { p.ProductName, m.MaterialName, p.ProductPrice });
        comment = "5. Результат выполнения запроса на выборку данных из таблиц Products и Materials c ценой изделия < 10: \r\n";
        Print(comment, products_materialsWithPrice.Take(5).ToList());

    }
    //Вставка данных
    static void Insert(UniqueProductsContext db)
    {
        // Создаём новый материал
        Material material = new()
        {
            MaterialName = "Пластик",
            MaterialDescript = "Чёрный"
        };
        // Создаём новое изделие
        Product product = new()
        {
            ProductName = "Труба для дымоотвода",
            ProductDescript = "Отличное качество",
            ProductWeight = 2,
            ProductDiameter = 10,
            ProductColor = "белый",
            MaterialId = 5,
            ProductPrice = 10
        };

        // Добавить в DbSet
        db.Materials.Add(material);
        db.Products.Add(product);
        // Сохранить изменения в базе данных
        db.SaveChanges();

        Console.WriteLine($"6. В таблицу Materials успешно добавлена запись:\n\tMaterialName:{material.MaterialName}\n\tMaterialDescript:{material.MaterialDescript}\n");
        Console.WriteLine($"7. В таблицу Products успешно добавлена запись:\n" +
            $"\tProductName:{product.ProductName},\n" +
            $"\tProductDescript:{product.ProductDescript}\n" +
            $"\tProductWeight:{product.ProductWeight}\n" +
            $"\tProductDiameter:{product.ProductDiameter}'\n"+
            $"\tProductColor:{product.ProductColor}\n"+
            $"\tMaterialId:{product.MaterialId}\n" +
            $"\tProductPrice:{product.ProductPrice}\n");
    }
    //Удаление даных
    static void Delete(UniqueProductsContext db)
    {
        var materialId = 4;
        //Запись из таблицы материалы
        var material = db.Materials.Where(m => m.MaterialId == materialId);

        //Записи из таблицы изделия
        var product = db.Products.OrderBy(p => p.ProductId).FirstOrDefault();

        //Удаление записей в таблице Materials и Products
        if (material != null) {
            db.Materials.RemoveRange(material); db.SaveChanges();
            Console.WriteLine($"\n8. Из таблицы Materials была успешно удалена запись с id={materialId}\n");
        }
        else { Console.WriteLine("\nВ таблице материалы нет такой записи!\n");}

        if (product != null) { 
            db.Products.Remove(product); db.SaveChanges();
            Console.WriteLine("\n9. Из таблицы Orders была успешно удалена первая запись\n");
        }
        else { Console.WriteLine("В таблице изделия нет такой записи!"); }
    }

    static void Update(UniqueProductsContext db)
    {
        var price = 50;
        //подлежащие обновлению записи в таблице Orders
        var order = db.Orders.Where(o => o.TotalPrice > price).FirstOrDefault();
        //обновление
        if (order != null)
        {
            order.OrderDate = DateTime.Today;
            // сохранить изменения в базе данных
            db.SaveChanges();
            Console.WriteLine($"\n10. Первый заказ в таблице Orders с общей стоимостью > {price} успешно обновлён: дата изменена на {order.OrderDate}");
        }
        else { Console.WriteLine("В таблице заказы нет такой записи!"); }
    }
}
