namespace server.Seeds;
using Microsoft.Data.Sqlite;

public static class DatabaseSeeder
{
    public static async Task PopulateSampleData(SqliteConnection connection)
    {
        // Database setup
        var createTableRoles = @"
            CREATE TABLE IF NOT EXISTS roles (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE
            )";

        var createTableUsers = @"
            CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT NOT NULL UNIQUE,
                email TEXT NOT NULL UNIQUE,
                password TEXT NOT NULL,
                role_id INTEGER,
                FOREIGN KEY(role_id) REFERENCES roles(id)
                    ON DELETE SET NULL
                    ON UPDATE CASCADE
            )";

        var createTableCategories = @"
            CREATE TABLE IF NOT EXISTS categories (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE
            )";

        var createTableProducts = @"
            CREATE TABLE IF NOT EXISTS products (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE,
                price INTEGER NOT NULL,
                category_id INTEGER,
                description TEXT,
                image_url TEXT,
                FOREIGN KEY(category_id) REFERENCES categories(id)
                    ON DELETE SET NULL
                    ON UPDATE CASCADE
            )";

        var createProductsView = @"
            CREATE VIEW IF NOT EXISTS products_view AS
            SELECT products.id, products.name, products.price, categories.name, products.description, products.image_url AS category
            FROM products
            LEFT JOIN categories ON products.category_id = categories.id
            ";

        var createUserView = @"
            CREATE VIEW IF NOT EXISTS user_view AS
            SELECT users.id, users.username, users.email, roles.name AS role
            FROM users
            LEFT JOIN roles ON users.role_id = roles.id
            ";

        var createTableOrders = @"
            CREATE TABLE IF NOT EXISTS orders (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                customer_id INTEGER,
                product_id INTEGER,
                quantity INTEGER NOT NULL,
                price INTEGER NOT NULL,
                total INTEGER GENERATED ALWAYS AS (quantity * price) VIRTUAL,
                created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY(customer_id) REFERENCES users(id)
                    ON DELETE SET NULL
                    ON UPDATE CASCADE,
                FOREIGN KEY(product_id) REFERENCES products(id)
                    ON DELETE SET NULL
                    ON UPDATE CASCADE
            )";

        var createTableOrderView = @"
            CREATE VIEW IF NOT EXISTS order_view AS
            SELECT orders.id, users.username, products.name as item, orders.quantity, orders.price, orders.total, orders.created_at
            FROM orders
            LEFT JOIN users ON orders.customer_id = users.id
            LEFT JOIN products ON orders.product_id = products.id
            ";

        try
        {
            await connection.OpenAsync();
            Console.WriteLine("**Connected to database**\n");

            Console.WriteLine("Enabling foreign keys...");
            using (var command = new SqliteCommand("PRAGMA foreign_keys = ON", connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Foreign keys enabled\n");

            Console.WriteLine("Creating table roles...if not exists");
            using (var command = new SqliteCommand(createTableRoles, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Creating table roles completed\n");

            Console.WriteLine("Creating table users...if not exists");
            using (var command = new SqliteCommand(createTableUsers, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Creating table users completed\n");

            Console.WriteLine("Creating table categories...if not exists");
            using (var command = new SqliteCommand(createTableCategories, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Creating table categories completed\n");

            Console.WriteLine("Creating table products...if not exists");
            using (var command = new SqliteCommand(createTableProducts, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Creating table products completed\n");

            Console.WriteLine("Creating table orders...if not exists");
            using (var command = new SqliteCommand(createTableOrders, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Creating table orders completed\n");

            Console.WriteLine("Creating view order_view...if not exists");
            using (var command = new SqliteCommand(createTableOrderView, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            Console.WriteLine("View order_view created\n");

            Console.WriteLine("Creating view user_view...if not exists");
            using (var command = new SqliteCommand(createUserView, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            Console.WriteLine("View user_view created\n");

            Console.WriteLine("Creating view products_view...if not exists");
            using (var command = new SqliteCommand(createProductsView, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            Console.WriteLine("View products_view created\n");

            Console.WriteLine("**Database setup completed**");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }




        Console.WriteLine("Populating database with sample data...\n");

        // Sample Roles
        var roles = new[] { "admin", "user" };
        foreach (var role in roles)
        {
            try
            {
                using var command = new SqliteCommand(
                    "INSERT INTO roles (name) VALUES ($name)",
                    connection
                );
                command.Parameters.AddWithValue("$name", role);
                await command.ExecuteNonQueryAsync();
                Console.WriteLine($"Added role: {role}");
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                Console.WriteLine($"Role {role} already exists");
            }
        }
        Console.WriteLine();

        // Sample Categories
        var categories = new[] { "Electronics", "Books", "Clothing", "Food" };
        foreach (var category in categories)
        {
            try
            {
                using var command = new SqliteCommand(
                    "INSERT INTO categories (name) VALUES ($name)",
                    connection
                );
                command.Parameters.AddWithValue("$name", category);
                await command.ExecuteNonQueryAsync();
                Console.WriteLine($"Added category: {category}");
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                Console.WriteLine($"Category {category} already exists");
            }
        }
        Console.WriteLine();

        // Sample Products
        var products = new[]
        {
        (Name: "Laptop", Price: 999, Category: "Electronics", Image_url: "https://cdn.thewirecutter.com/wp-content/media/2024/11/BEST-LAPTOPS-PHOTO-VIDEO-EDITING-2048px-6.jpg"),
        (Name: "Smartphone", Price: 499, Category: "Electronics", Image_url: "https://cdn.thewirecutter.com/wp-content/media/2024/05/smartphone-2048px-1013.jpg"),
        (Name: "The Great Gatsby", Price: 10, Category: "Books", Image_url: "https://m.media-amazon.com/images/I/71V1cA2fiZL._AC_UF1000,1000_QL80_.jpg"),
        (Name: "1984", Price: 12, Category: "Books", Image_url: "https://cdn.ibpbooks.com/images/sdf/1985.jpg"),
        (Name: "T-Shirt", Price: 20, Category: "Clothing", Image_url: "https://nobero.com/cdn/shop/files/aloe-green_465e61f9-b1b2-4a8d-9cf5-339150cfed64.jpg?v=1724760910"),
        (Name: "Jeans", Price: 45, Category: "Clothing", Image_url: "https://images.only.com/15327177/4544723/001/only-onlharleylwbaggylooseazdnmfw-blaat.jpg?v=3bffdc08dda35e47ba382069cfd38327&format=webp&width=1280&quality=90&key=25-0-3"),
        (Name: "Pizza", Price: 15, Category: "Food", Image_url: "https://images.arla.com/recordid/1B752FAF-42F2-4347-9E8B99C5E6B782A9/pizza.jpg?width=1200&height=630&mode=crop&format=jpg"),
        (Name: "Burger", Price: 8, Category: "Food", Image_url: "https://dynamic-media-cdn.tripadvisor.com/media/photo-o/2d/5f/44/34/smashed-patty-burger.jpg?w=900&h=500&s=1"),
        (Name: "Headphones", Price: 50, Category: "Electronics", Image_url: "https://zildjian.com/cdn/shop/files/Zildjian-ALCHEM-E-Perfect-Tune-Headphones-Black-Product-Image-1_f4fa0315-beee-4fa5-952d-ed141c448aef.jpg?v=1724252909&width=1946"),
        (Name: "Mouse", Price: 20, Category: "Electronics", Image_url: "https://us.v-cdn.net/6036147/uploads/67SAKXLFNL4I/l-10-8-1-1200x675.jpg"),
        (Name: "Keyboard", Price: 30, Category: "Electronics", Image_url: "https://preview.redd.it/in-search-of-a-vintage-keyboard-the-ibm-model-m-v0-4opfq1h4ikra1.jpg?width=1350&format=pjpg&auto=webp&s=dbcf010d70a9b1f00ee1b85ca6ab6bc5f412c07e"),
        (Name: "The Catcher in the Rye", Price: 15, Category: "Books", Image_url: "https://m.media-amazon.com/images/I/91fQEUwFMyL.jpg"),
        (Name: "To Kill a Mockingbird", Price: 18, Category: "Books", Image_url: "https://m.media-amazon.com/images/I/71FxgtFKcQL._AC_UF1000,1000_QL80_.jpg"),
        (Name: "Dress", Price: 35, Category: "Clothing", Image_url: "https://styleshops.com.ph/cdn/shop/files/unica-x-wwf-dresses-wistoria-dress-42503827357927.jpg?v=1719768012"),
        (Name: "Sweater", Price: 25, Category: "Clothing", Image_url: "https://m.media-amazon.com/images/I/81L5shykO1L._AC_UY1000_.jpg"),
        (Name: "Pasta", Price: 12, Category: "Food", Image_url: "https://cdn.apartmenttherapy.info/image/upload/f_jpg,q_auto:eco,c_fill,g_auto,w_1500,ar_1:1/k%2FPhoto%2FRecipes%2F2023-01-Caramelized-Tomato-Paste-Pasta%2F06-CARAMELIZED-TOMATO-PASTE-PASTA-039"),
        (Name: "Salad", Price: 10, Category: "Food", Image_url: "https://images.ctfassets.net/e8gvzq1fwq00/73CwiO6e5qknz5D748ZJwj/c48bcf07f527d91b09f684cb6484975c/Sallad_med_stekt_tofu__avokado__bl__b__r_och_honungsvin__grett_desktop.png"),
        (Name: "Monitor", Price: 150, Category: "Electronics", Image_url: "https://www.livemint.com/lm-img/img/2024/11/21/600x338/best_pc_monitor_1732218689673_1732218701023.jpg"),
        (Name: "Tablet", Price: 80, Category: "Electronics", Image_url: "https://m.media-amazon.com/images/I/71pMKb47muL._AC_SL1500_.jpg"),
        (Name: "War and Peace", Price: 20, Category: "Books", Image_url: "https://cdn.kobo.com/book-images/3ac03eac-d437-47e3-9b15-52542edabd56/1200/1200/False/war-and-peace-34.jpg"),
        (Name: "The Odyssey", Price: 22, Category: "Books", Image_url: "https://mpd-biblio-covers.imgix.net/9781429973427.jpg"),
        (Name: "Jacket", Price: 55, Category: "Clothing", Image_url: "https://m.media-amazon.com/images/I/8118R-2U6wL._AC_SL1500_.jpg"),
        (Name: "Shoes", Price: 40, Category: "Clothing", Image_url: "https://miro.medium.com/v2/resize:fit:403/1*6sTlqGOGW5XR12pAnaWTJQ.jpeg"),
        (Name: "Ice Cream", Price: 5, Category: "Food", Image_url: "https://bitzngiggles.com/wp-content/uploads/2020/02/Rainbow-Ice-Cream-14-copy-500x500.jpg"),
        (Name: "Cake", Price: 18, Category: "Food", Image_url: "https://recipesblob.oetker.co.uk/assets/46b664a502ce4ebdb241e6667ce789b7/360x400/pinata-rainbow-cake.webp"),
        (Name: "Printer", Price: 100, Category: "Electronics", Image_url: "https://www.lifewire.com/thmb/_GOO_BLQmZ9z8skP1R1_ZMBJ5vY=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/printer-649004582-31623906dc38475bba79c63b0d18d761.jpg"),
        (Name: "Camera", Price: 200, Category: "Electronics", Image_url: "https://media.istockphoto.com/id/185278433/photo/black-digital-slr-camera-in-a-white-background.jpg?s=612x612&w=0&k=20&c=OOCbhvOF0W-eVhhrm-TxbgLfbKhFfs4Lprjd7hiQBNU="),
        (Name: "The Divine Comedy", Price: 25, Category: "Books", Image_url: "https://notebookm.com/wp-content/uploads/2024/06/divine-comedy-cover.jpg")
    };

        foreach (var product in products)
        {
            try
            {
                using var command = new SqliteCommand(@"
                INSERT INTO products (name, price, category_id, description, image_url)
                SELECT $name, $price, id, $description, $image_url
                FROM categories
                WHERE name = $category",
                    connection
                );
                command.Parameters.AddWithValue("$name", product.Name);
                command.Parameters.AddWithValue("$price", product.Price);
                command.Parameters.AddWithValue("$category", product.Category);
                command.Parameters.AddWithValue("$description", $"This is {product.Name} in the {product.Category} category");
                command.Parameters.AddWithValue("$image_url", product.Image_url);
                await command.ExecuteNonQueryAsync();
                Console.WriteLine($"Added product: {product.Name} (${product.Price}) in {product.Category}");
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                Console.WriteLine($"Product {product.Name} already exists");
            }
        }
        Console.WriteLine();

        // Sample Users
        var users = new[]
        {
        (Username: "admin", Email: "admin@admin.com", Password: "admin123", Role: "admin"),
        (Username: "john", Email: "john@email.com", Password: "john123", Role: "user"),
        (Username: "jane", Email: "jane@email.com", Password: "jane123", Role: "user")
    };

        foreach (var user in users)
        {
            try
            {
                using var command = new SqliteCommand(@"
                INSERT INTO users (username, email, password, role_id)
                SELECT $username, $email, $password, id
                FROM roles
                WHERE name = $role",
                    connection
                );
                command.Parameters.AddWithValue("$username", user.Username);
                command.Parameters.AddWithValue("$email", user.Email);
                command.Parameters.AddWithValue("$password", user.Password);
                command.Parameters.AddWithValue("$role", user.Role);
                await command.ExecuteNonQueryAsync();
                Console.WriteLine($"Added user: {user.Username} with role {user.Role}");
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                Console.WriteLine($"User {user.Username} already exists");
            }
        }

        Console.WriteLine("\nSample data population completed!");
    }
}