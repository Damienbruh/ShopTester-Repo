import { BrowserRouter, Route, Routes } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import './App.css';
import { UserProvider } from './context/UserContext';
import Products from './components/Products';
import Details from './components/Details';
import AdminDashboard from './components/AdminDashboard';
import ProtectedRoute from './util/ProtectedRoute';
import Profile from './components/Profile';
import { CartProvider } from './context/CartContext';
import Cart from './components/Cart';

function App() {

  return (
    <UserProvider>
      <CartProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<Layout />} >
              <Route index element={<Home />} />
              <Route path='/shop' element={<Products />} />
              <Route path='/shop/product/:id' element={<Details />} />
              <Route path='/admin' element={<ProtectedRoute><AdminDashboard /></ProtectedRoute>} />
              <Route path='/profile' element={<Profile />} />
              <Route path='/cart' element={<Cart />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </CartProvider>
    </UserProvider>
  )
}

export default App
