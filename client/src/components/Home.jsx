import { useNavigate } from 'react-router-dom';

export default function Home() {
    const navigate = useNavigate();
    return (
        <div style={{ textAlign: 'center', padding: '20px', fontFamily: 'Arial, sans-serif', backgroundColor: '#f0f8ff' }}>
            <h1 style={{ color: '#ff4500', fontSize: '3em' }}>Welcome Kasper! CI/CD is awesome!</h1>
            <p style={{ color: '#2e8b57', fontSize: '1.2em' }}>Your one-stop shop for all your testing needs.</p>
            <img src="https://dl.the-web.co.jp/wp-content/uploads/2021/12/THE-SHOP-PICT1%EF%BC%88TOKYO-_-main%EF%BC%89.jpg" alt="TestShopper Logo" style={{ borderRadius: '20%', margin: '20px 0', width: '70%' }} />
            <p style={{ color: '#2e8b57', fontSize: '1.2em' }}>Browse our categories and find the best products for your testing requirements.</p>
            <p style={{ color: '#ff4500', fontSize: '1.2em' }}>Note: This is a members-only webshop. Please log in to access exclusive deals and offers.</p>
            <p style={{ color: '#2e8b57', fontSize: '1.2em', margin: '20px 0' }}>
                Funny story: Once upon a time, a duck decided to learn how to code. It waddled its way to the nearest computer and started typing away with its webbed feet. To everyone's surprise, the duck became a coding prodigy and created an app that quacked every time there was a bug in the code. Developers loved it, and the duck became famous in the tech world. Now, you too can benefit from the best testing tools available. Happy shopping!
            </p>
            <button
                onClick={() => navigate('/shop')}
                style={{ padding: '10px 20px', fontSize: '1em', color: '#fff', backgroundColor: '#ff4500', border: 'none', borderRadius: '5px', cursor: 'pointer' }}
            >
                Start Shopping
            </button>
        </div>
    )
}