
using Microsoft.Data.Sqlite;


namespace TestProject1
{
    public class OrderCreationLogicTests // Simple order creation logic
    {
        [Fact]
        public void OrderTotal_IsCalculatedCorrectly()
        {
            var quantity = 2;
            var price = 15;
            var total = quantity * price;
            Assert.Equal(30, total);
        }
    }

    public class SqlOrderTests // Order creation using SQL
    {
        [Fact]
        public async Task InsertOrderIntoDb_Succeeds()
        {
            var schema = @"
                CREATE TABLE orders (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    customer_id INTEGER NOT NULL,
                    product_id INTEGER NOT NULL,
                    quantity INTEGER NOT NULL,
                    price REAL NOT NULL
                );
            ";

            using var connection = TestDatabase.CreateInMemoryDatabase(schema);
            var insert = connection.CreateCommand();
            insert.CommandText = "INSERT INTO orders (customer_id, product_id, quantity, price) VALUES (1, 2, 3, 10.5);";
            var result = await insert.ExecuteNonQueryAsync();

            Assert.Equal(1, result);
        }
    }

    public class ProductLogicTests // Product creation logic check
    {
        [Fact]
        public void CanCreateProduct()
        {
            var name = "SimpleProduct";
            var price = 10;
            var categoryId = 1;

            Assert.False(string.IsNullOrWhiteSpace(name));
            Assert.True(price > 0);
            Assert.True(categoryId > 0);
        }
    }

    public class SqlProductTests // SQL Product create & delete test
    {
        [Fact]
        public async Task CreateThenDeleteProduct_Succeeds()
        {
            var schema = @"
                CREATE TABLE products (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    price REAL,
                    category_id INTEGER
                );
            ";

            using var connection = TestDatabase.CreateInMemoryDatabase(schema);

            var insert = connection.CreateCommand();
            insert.CommandText = "INSERT INTO products (name, price, category_id) VALUES ('Test', 99.9, 1);";
            await insert.ExecuteNonQueryAsync();

            var delete = connection.CreateCommand();
            delete.CommandText = "DELETE FROM products WHERE name = 'Test';";
            var rows = await delete.ExecuteNonQueryAsync();

            Assert.Equal(1, rows);
        }
    }

    public class OrderHistoryLogicTests // Logic-only test for total spent
    {
        [Fact]
        public void TotalSpent_IsCalculatedCorrectly()
        {
            var orders = new[]
            {
                new { Quantity = 2, Price = 10 },
                new { Quantity = 1, Price = 20 }
            };

            var total = orders.Sum(o => o.Quantity * o.Price);
            Assert.Equal(40, total);
        }
    }

    public class SqlOrderHistoryTests // SQL test for order total
    {
        [Fact]
        public async Task CalculateTotalFromOrderView_Works()
        {
            var schema = @"
                CREATE TABLE order_view (
                    id INTEGER PRIMARY KEY,
                    username TEXT NOT NULL,
                    product_id INTEGER NOT NULL,
                    quantity INTEGER NOT NULL,
                    price REAL NOT NULL
                );
            ";

            var seed = @"
                INSERT INTO order_view (id, username, product_id, quantity, price)
                VALUES (1, 'john', 1, 2, 15.0),
                       (2, 'john', 2, 1, 20.0);
            ";

            using var connection = TestDatabase.CreateInMemoryDatabase(schema, seed);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SUM(quantity * price) FROM order_view WHERE username = 'john';";

            var result = await cmd.ExecuteScalarAsync();
            var total = Convert.ToDecimal(result);

            Assert.Equal(50.0m, total);
        }
    }

    public static class TestDatabase
    {
        public static SqliteConnection CreateInMemoryDatabase(string schemaSql, string seedSql = "")
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = schemaSql;
            cmd.ExecuteNonQuery();

            if (!string.IsNullOrWhiteSpace(seedSql))
            {
                var seedCmd = connection.CreateCommand();
                seedCmd.CommandText = seedSql;
                seedCmd.ExecuteNonQuery();
            }

            return connection;
        }
    }
}
