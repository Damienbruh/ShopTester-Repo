import { useEffect, useState } from 'react';
import { useUser } from '../context/UserContext';

export default function Profile() {
    const { user } = useUser();
    const [orders, setOrders] = useState([]);
    const [totalSpent, setTotalSpent] = useState(0);

    useEffect(() => {
        const fetchOrders = async () => {
            try {
                const response = await fetch(`/api/user/${user.id}/orders`, {
                    credentials: 'include'
                });
                if (response.ok) {
                    const ordersData = await response.json();
                    setOrders(ordersData);
                    //console.log('Orders:', ordersData);
                    let result = 0;
                    ordersData.forEach(order => {
                        result += order.total;
                    });
                    setTotalSpent(result);
                    //console.log('Total spent:', totalSpent);
                }
                if (!response.ok) {
                    //console.log('No orders found');
                }
            } catch (error) {
                console.error('Orders fetch failed:', error);
            }
        };

        fetchOrders();
    }
        , [totalSpent, user.id]);

    if (!user) {
        return <div>Loading...</div>
    }

    return (
        <div>
            <h2>Hi {user.username.charAt(0).toUpperCase() + user.username.slice(1)}!</h2>
            <p>Your email: {user.email}</p>
            <p>Your role: {user.role}</p>
            <p>Total amount spent: {totalSpent}$</p>
            <div>
                <h2>Order history:</h2>
                {orders.length < 1 ? <p>No orders!</p> :
                    <ul>
                        {orders.map(order => (
                            <li key={order.id} id={order.id}>
                                <p>Order ID: {order.id}</p>
                                <p>Order Date: {new Intl.DateTimeFormat('se-SE', { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false }).format(order.created_at)}</p>
                                <p>Order Product: {order.productName}</p>
                                <p>Order Quantity: {order.quantity}</p>
                                <p>Order Total: ${order.total}</p>
                                <p>-------------------------------------</p>
                            </li>
                        ))}
                    </ul>
                }
            </div>
        </div>
    )
}