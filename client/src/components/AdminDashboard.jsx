import { useState } from 'react';

export default function AdminDashboard() {

    const [users, setUsers] = useState([]);
    const [orders, setOrders] = useState([]);
    const [products, setProducts] = useState([]);
    const [productName, setProductName] = useState('');
    const [productPrice, setProductPrice] = useState(0);
    const [productCategoryId, setProductCategoryId] = useState(0);
    const [productDescription, setProductDescription] = useState('');
    const [productImage, setProductImage] = useState('');
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [confirmationMessage, setConfirmationMessage] = useState('');

    const handleDialogOpen = () => {
        setIsDialogOpen(true);
    };

    const handleDialogClose = () => {
        setIsDialogOpen(false);
    };

    const fetchUsers = async () => {
        const response = await fetch('api/users', { credentials: 'include' });
        const data = await response.json();
        setUsers(data);
    }

    const fetchOrders = async () => {
        const response = await fetch('api/orders', { credentials: 'include' });
        const data = await response.json();
        setOrders(data);
    }

    const fetchProducts = async () => {
        const response = await fetch('api/products', { credentials: 'include' });
        const data = await response.json();
        setProducts(data);
    }

    const handleAddProduct = async (e) => {
        e.preventDefault();
        const response = await fetch('api/products', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                name: productName,
                price: productPrice,
                categoryId: productCategoryId,
                description: productDescription,
                image_url: productImage
            }),
            credentials: 'include'
        });
        if (response.ok) {
            fetchProducts();
            setConfirmationMessage('Product added successfully!');
            setTimeout(() => setConfirmationMessage(''), 5000);
        }
        setIsDialogOpen(false);
        setProductName('');
        setProductPrice(0);
        setProductCategoryId(0);
        setProductDescription('');
        setProductImage('');
    }

    const handleChangeProduct = async () => {
        // TODO
    }

    const handleDeleteOrder = async (id) => {
        const response = await fetch(`api/orders/${id}`, {
            method: 'DELETE',
            credentials: 'include'
        });
        if (response.ok) {
            fetchOrders();
        }
    }

    const handleDeleteUser = async (id) => {
        const response = await fetch(`api/users/${id}`, {
            method: 'DELETE',
            credentials: 'include'
        });
        if (response.ok) {
            fetchUsers();
        }
    }

    const handleDeleteProduct = async (id) => {
        const response = await fetch(`api/products/${id}`, {
            method: 'DELETE',
            credentials: 'include'
        });
        if (response.ok) {
            fetchProducts();
        }
    }

    return (
        <>
            <div>
                <h1>Admin Dashboard</h1>
                {confirmationMessage && (
                    <div id='confirmation' style={{
                        padding: '10px',
                        margin: '10px 0',
                        backgroundColor: confirmationMessage.includes('Error') || confirmationMessage.includes('Failed')
                            ? '#ffebee'
                            : '#e8f5e9',
                        color: confirmationMessage.includes('Error') || confirmationMessage.includes('Failed')
                            ? '#c62828'
                            : '#2e7d32',
                        border: '1px solid',
                        borderColor: confirmationMessage.includes('Error') || confirmationMessage.includes('Failed')
                            ? '#ef5350'
                            : '#66bb6a',
                        borderRadius: '4px',
                        position: 'fixed',
                        top: '20px',
                        right: '20px',
                        zIndex: 1000
                    }}>
                        {confirmationMessage}
                    </div>
                )}
                <p>This is where you can add, change and delete things to and from the database like products.
                    You can fetch all users, fetch all orders. And you can filter through your data</p>
                <div>
                    <div style={{ display: 'flex', alignItems: 'center' }}>
                        <h2 style={{ marginBottom: '0px' }}>Users</h2>
                        <button style={{ margin: '10px', backgroundColor: 'darkgreen' }} onClick={fetchUsers}>Fetch Users</button>
                    </div>
                    <ul>
                        {users.map(user => <li key={user.id}>
                            <div style={{ border: '1px solid gray', padding: '10px', display: 'flex', alignItems: 'center' }}>
                                <div style={{ flex: 1 }}>
                                    <p>{user.email}</p>
                                </div>
                                <button style={{ margin: '10px', backgroundColor: 'darkred' }} onClick={() => handleDeleteUser(user.id)}>Delete user</button>
                            </div>
                        </li>)}
                    </ul>

                    <div style={{ display: 'flex', alignItems: 'center' }}>
                        <h2 style={{ margin: '0px' }}>Orders</h2>
                        <button style={{ margin: '10px', backgroundColor: 'darkgreen' }} onClick={fetchOrders}>Fetch Orders</button>
                    </div>
                    <ul>
                        {orders.map(order => <li key={order.id}><div id={order.id} style={{ border: '1px solid gray', padding: '10px', display: 'flex', alignItems: 'center' }}>
                            <div style={{ flex: 1 }}>
                                <p>ID: {order.id}</p>
                                <p>Username: {order.username}</p>
                                <p>Product: {order.productName}</p>
                                <p>Quantity: {order.quantity}</p>
                                <p>Price: {order.price}</p>
                                <p>Date: {Date(order.created_at)}</p>
                            </div>
                            <button id='button-delete' style={{ margin: '10px', backgroundColor: 'darkred' }} onClick={() => handleDeleteOrder(order.id)}>Delete order</button>
                        </div></li>)}
                    </ul>

                    <div style={{ display: 'flex', alignItems: 'center' }}>
                        <h2 style={{ margin: '0px' }}>Products</h2>
                        <button style={{ margin: '10px', backgroundColor: 'darkgreen' }} onClick={handleDialogOpen}>Add Product</button>
                        <button style={{ margin: '10px', backgroundColor: 'darkgreen' }} onClick={handleChangeProduct}>Change Product</button>
                        <button style={{ margin: '10px', backgroundColor: 'darkgreen' }} onClick={fetchProducts}>Fetch Products</button>
                    </div>
                    <ul>
                        {products.map(product => <li key={product.id}><div id={product.name} style={{ border: '1px solid gray', padding: '10px', display: 'flex', alignItems: 'center' }}>
                            <div style={{ flex: 1 }}>
                                <p>{product.name}</p>
                                <p>{product.price}</p>
                                <p>{product.description}</p>
                            </div>
                            <img style={{ width: '70px', marginLeft: '10px' }} src={product.image_url} alt="" />
                            <button style={{ margin: '10px', backgroundColor: 'darkred' }} onClick={() => handleDeleteProduct(product.id)}>Delete product</button>
                        </div>
                        </li>)}
                    </ul>
                </div>
            </div>
            {
                isDialogOpen && (
                    <div className="dialog-overlay">
                        <div className="dialog">
                            <h2>Login</h2>
                            <form id="add-product" onSubmit={handleAddProduct}>
                                <label>
                                    Product Name:
                                    <input
                                        type="name"
                                        name="name"
                                        value={productName}
                                        onChange={(e) => setProductName(e.target.value)}
                                        required
                                    />
                                </label>
                                <label>
                                    Price:
                                    <input
                                        type="price"
                                        name="price"
                                        value={productPrice}
                                        onChange={(e) => setProductPrice(e.target.value)}
                                        required
                                    />
                                </label>
                                <label>
                                    Category ID:
                                    <input
                                        type="categoryId"
                                        name="categoryId"
                                        value={productCategoryId}
                                        onChange={(e) => setProductCategoryId(e.target.value)}
                                        required
                                    />
                                </label>
                                <label>
                                    Description:
                                    <input
                                        type="description"
                                        name="description"
                                        value={productDescription}
                                        onChange={(e) => setProductDescription(e.target.value)}
                                        required
                                    />
                                </label>
                                <label>
                                    Image URL:
                                    <input
                                        type="text"
                                        name="text"
                                        value={productImage}
                                        onChange={(e) => setProductImage(e.target.value)}
                                    />
                                </label>
                                <div className="button-group">
                                    <button type="button" onClick={handleDialogClose}>Close</button>
                                    <button type="submit">Submit</button>
                                </div>
                            </form>
                        </div>
                    </div>
                )
            }
        </>
    );
}